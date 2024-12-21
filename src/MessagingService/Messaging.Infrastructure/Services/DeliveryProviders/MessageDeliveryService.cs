using Messaging.Infrastructure.Contracts.Common.Enums;
using Messaging.Infrastructure.Contracts.InstantMessage.Commands;
using Messaging.Infrastructure.Contracts.Message.Commands;
using Messaging.Infrastructure.Domain;
using Messaging.Infrastructure.Services.DeliveryProviders.EmailDelivery;
using Messaging.Infrastructure.Services.DeliveryProviders.EmailDelivery.Contracts;
using Messaging.Infrastructure.Services.DeliveryProviders.SMSDelivery;
using Messaging.Infrastructure.Services.DeliveryProviders.SMSDelivery.Contracts;
using Messaging.Infrastructure.Services.Persistence;

namespace Messaging.Infrastructure.Services.DeliveryProviders;

public class MessageDeliveryService : IMessageDeliveryService
{
    private readonly ISMSProviderService _smsProviderService;
    private readonly IEmailProviderService _emailProviderService;
    private readonly IMessagingRepository _messagingRepository;

    public MessageDeliveryService(ISMSProviderService smsProviderService, IEmailProviderService emailProviderService, IMessagingRepository messagingRepository)
    {
        _smsProviderService = smsProviderService;
        _emailProviderService = emailProviderService;
        _messagingRepository = messagingRepository;
    }

    public async Task<SendInstantMessageCommandResult> DeliverQueuedMessageAsync(SendInstantMessageCommand command, CancellationToken cancellationToken)
    {

        //this method should be threadsafe and i need to lock the repository actions
        return await Send(command, cancellationToken);

    }

    public async Task<SendBulkMessageResult> SendBulkMessageAsync(SendBulkMessageCommand command, CancellationToken cancellationToken)
    {
        var models = new List<BulkInstanceMessage>();
        
        foreach(var to in command.PhoneNumbers)
        {
            models.Add(new BulkInstanceMessage()
            {
                Text = command.Text,
                Sent = false,
                To = to,
                OperationId = Guid.NewGuid(),
            });
        }

        models = await _messagingRepository.AddRangeBulkInstanceMessageAsync(models);


        await SendBulk(models, command.DeliveryMethodType, cancellationToken);

        await _messagingRepository.UpdateRangeBulkInstanceMessageAsync(models);

        var result = new SendBulkMessageResult();

        foreach(var model in models)
        {
            result.Status.Add(model.To, model.Sent);
        }

        return result;
    }

    public async Task<SendBulkMessageResult> RetrySendBulkMessage(RetryBulkMessageCommand command, CancellationToken cancellationToken)
    {
        var models = await _messagingRepository.GetNotSentBulkMessages();

        await SendBulk(models, command.DeliveryMethod, cancellationToken);

        await _messagingRepository.UpdateRangeBulkInstanceMessageAsync(models);

        var result = new SendBulkMessageResult();

        foreach (var model in models)
        {
            result.Status.Add(model.To, model.Sent);
        }

        return result;
    }

    public async Task<SendInstantMessageCommandResult> SendInstantMessageAsync(SendInstantMessageCommand command, CancellationToken cancellationToken)
    {
        // todo: persistence logic needed here
        var result = await Send(command, cancellationToken);
        
        if (result.Sent)
        {
            Message message = new Message() { Body = command.Body, DeliveryMethod = command.DeliveryMethod, Queued = false, Recipients = new List<Recipient>() { new Recipient(command.RecipientKey, true, false) }, Sender = command.Sender };
            await _messagingRepository.AddMessageAsync(message, CancellationToken.None); //dont want to cancel
        }

        return result;

    }

    private async Task<SendInstantMessageCommandResult> Send(SendInstantMessageCommand command, CancellationToken cancellationToken) //change this later
    {
        switch (command.DeliveryMethod)
        {
            case DeliveryMethodType.SMS:
                
                SendSMSResponse sendSMSResponse = await _smsProviderService.SendSMS(new SendSMSRequest(command.RecipientKey, command.Body, command.Sender),cancellationToken);
                return new SendInstantMessageCommandResult(sendSMSResponse.IsSuccessfull, false);

            case DeliveryMethodType.Email:
               
                var result = await _emailProviderService.SendEmail(new SendEmailRequest(command.RecipientKey, null, command.Body, null), cancellationToken);
                return new SendInstantMessageCommandResult(result.Sent, false, result.Message);
                
            default:
                throw new Exception("invalid delivery method");
        }
    }

    private async Task<List<BulkInstanceMessage>> SendBulk(List<BulkInstanceMessage> bulkInstances, DeliveryMethodType deliveryMethod , CancellationToken cancellationToken = default)
    {
        switch (deliveryMethod)
        {
            case DeliveryMethodType.SMS:

                

                foreach(var message in bulkInstances)
                {
                    SendSMSResponse sendSMSResponse = await _smsProviderService.SendBulkInstanceSMS(new SendSMSRequest(message.To, message.Text), cancellationToken);
                    message.Sent = sendSMSResponse.IsSuccessfull;
                }

                return bulkInstances;

            case DeliveryMethodType.Email:

                throw new Exception("instant bulk email not supported yet");

            default:
                throw new Exception("invalid delivery method");
        }
    }
}
