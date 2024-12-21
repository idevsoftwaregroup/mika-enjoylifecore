using Messaging.Infrastructure.Contracts.Message.Commands;
using Messaging.Infrastructure.Domain;
using Messaging.Infrastructure.Services.DeliveryProviders;
using Messaging.Infrastructure.Services.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Messaging.Infrastructure.Services.MessageQueue;

public class MessageConsumptionEventHandler : IMessageConsumptionEventHandler
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<MessageConsumptionEventHandler> _logger;

    public MessageConsumptionEventHandler(IServiceProvider serviceProvider, ILogger<MessageConsumptionEventHandler> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    //public MessageConsumptionEventHandler(IMessageDeliveryService messageDeliveryService, IMessagingRepository messagingRepository)
    //{
    //    _messageDeliveryService = messageDeliveryService;
    //    _messagingRepository = messagingRepository;
    //}

    public async Task Handle(object? sender, BasicDeliverEventArgs args, IModel channel, CancellationToken cancellationToken)
    {
        using (var scope = _serviceProvider.CreateScope())
        {

            var bytes = args.Body.ToArray();
            var json = Encoding.UTF8.GetString(bytes);
            var messageId = JsonSerializer.Deserialize<int>(json);
            var messagingRepository = scope.ServiceProvider.GetRequiredService<IMessagingRepository>();

            var message = await messagingRepository.GetMessageAsync(messageId, cancellationToken);

            if (message is null)
            {
                channel.BasicNack(args.DeliveryTag, false, false);
                _logger.LogError($"message with Id: {messageId} does not exist.");
                return;
            }
                
            await ConsumeMessage(message, messagingRepository, scope.ServiceProvider.GetRequiredService<IMessageDeliveryService>(),cancellationToken);

            if (message.Recipients.All(r => r.Sent))
            {
                message.Queued = false;
                await messagingRepository.UpdateMessage(message, cancellationToken);
                channel.BasicAck(args.DeliveryTag, false);
            }

            
        }

    }

    private async Task ConsumeMessage(Message message, IMessagingRepository messagingRepository, IMessageDeliveryService messageDeliveryService,CancellationToken cancellationToken)
    {
        #region concurrnet operations
        // Create a list to store tasks
        var deliveryTasks = new List<Task<(Recipient, SendInstantMessageCommandResult)>>();

        foreach (Recipient recipient in message.Recipients)
        {
            if (!recipient.Sent)
            {
                var deliveryTask = Task.Run(async () =>
                {
                    SendInstantMessageCommandResult result;
                    
                    try
                    {
                         result = await DeliverMessage(message, recipient, messageDeliveryService,cancellationToken);
                    }
                    catch(Exception ex) // should i nack the message here ? or try again next time ? what if a certain recipient is just impossible ?
                    {
                        _logger.LogError(ex.Message);

                        _logger.LogError(ex.InnerException?.Message);

                        result = new SendInstantMessageCommandResult(false, false, "an error occured while sending the message");
                    }

                    return (recipient, result);

                },cancellationToken);

                deliveryTasks.Add(deliveryTask);
            }
        }

        // Wait for all delivery tasks to complete
        await Task.WhenAll(deliveryTasks);
        #endregion

        var messageRecipients = message.Recipients.ToList();
        // Update recipients based on the delivery results
        foreach (var (recipient, result) in deliveryTasks.Select(t => t.Result))
        {
            // Update recipient properties here
            //messageRecipients = messageRecipients.Select<Recipient,Recipient>(r => { return r == recipient ? (r with { Sent = result.Sent, Delivered = result.Delivered }) : r; });
            int index = messageRecipients.IndexOf(recipient);

            var updatedRecipient = recipient with
            {
                Sent = result.Sent,
                Delivered = result.Delivered
            };

            messageRecipients[index] = updatedRecipient;
        }

        message.UpdateRecipients(messageRecipients);

        await messagingRepository.UpdateMessage(message, cancellationToken);

    }

    private async Task<SendInstantMessageCommandResult> DeliverMessage(Message message, Recipient recipient, IMessageDeliveryService messageDeliveryService, CancellationToken cancellationToken)
    {
        return await messageDeliveryService.DeliverQueuedMessageAsync(new SendInstantMessageCommand(recipient.key, message.DeliveryMethod, message.Body, message.Sender), cancellationToken);
    }
}
