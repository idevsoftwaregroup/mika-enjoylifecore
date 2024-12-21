using Messaging.Infrastructure.Contracts.Message.Commands;
using Messaging.Infrastructure.Contracts.QueueMessage.Commands;
using Messaging.Infrastructure.Domain;
using Messaging.Infrastructure.Services.DeliveryProviders;
using Messaging.Infrastructure.Services.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Messaging.Infrastructure.Services.MessageQueue;

public class MessageQueueService : IMessageQueueService
{
    private readonly MessageQueueSettings _settings;
    private readonly RabbitConnectionProvider _connectionProvider;
    private readonly IMessagingRepository _messagingRepository;
    private readonly IMessageDeliveryService _messageDeliveryService;
    private readonly IServiceScopeFactory _serviceProvider;
    private readonly IMessageConsumptionEventHandler messageConsumptionEventHandler;
    public MessageQueueService(IOptions<MessageQueueSettings> settingsOptions,
                               RabbitConnectionProvider connectionProvider,
                               IMessagingRepository messagingRepository,
                               IMessageDeliveryService messageDeliveryService,
                               IServiceScopeFactory serviceProvider,
                               IMessageConsumptionEventHandler messageConsumptionEventHandler)
    {
        _settings = settingsOptions.Value;
        _connectionProvider = connectionProvider;
        _messagingRepository = messagingRepository;
        _messageDeliveryService = messageDeliveryService;
        _serviceProvider = serviceProvider;
        this.messageConsumptionEventHandler = messageConsumptionEventHandler;
    }

    public async Task<DequeueMessageCommandResult> DequeueMessageAsync(DequeueMessageCommand command, CancellationToken cancellationToken)
    {
        IModel channel = _connectionProvider.GetChannel();
        channel.BasicQos(0, command.numberOfMessagesDequeued, false);

        var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        var consumer = new EventingBasicConsumer(channel);

        consumer.Received += async (sender, args) =>
        {
            try
            {
                await messageConsumptionEventHandler.Handle(sender, args, channel, cancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
                // Handle cancellation gracefully ???
            }
        };

        var consumerTag = channel.BasicConsume(_settings.QueueName, false, consumer);

        try
        {
            await Task.Delay(command.listentingTime, cancellationToken);
        }
        catch (TaskCanceledException)
        {
            // Handle cancellation ???
        }
        finally
        {
            cancellationTokenSource.Cancel();
            channel.Close();
        }

        return new DequeueMessageCommandResult();
    }





    public async Task<EnqueueMessageCommandResult> EnqueueMessageAsync(EnqueueMessageCommand command, CancellationToken cancellationToken)
    {
        Message message = new Message
        {
            Body = command.Body,
            DeliveryMethod = command.DeliveryMethod,
            Queued = false,
            Sender = command.Sender,
        };

        message.UpdateRecipients(command.RecipientKeys.Select(x => new Recipient(x, false, false)).ToList());

        await _messagingRepository.AddMessageAsync(message, cancellationToken);

        try
        {


            IModel channel = _connectionProvider.GetChannel();
            message.Queued = true;

            string json = JsonSerializer.Serialize(message.Id);
            byte[] bytes = Encoding.UTF8.GetBytes(json);

            channel.BasicPublish(_settings.ExchangeName, _settings.RoutingKey, null, bytes);

            channel.Close();
            //channel.Dispose();

        }
        catch (Exception ex) // not good should change its just for starter version
        {
            return new EnqueueMessageCommandResult(false, 0, ex.Message);
        }

        await _messagingRepository.UpdateMessage(message, cancellationToken);

        return new EnqueueMessageCommandResult(true, message.Id, null);
    }

}
