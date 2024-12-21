using Messaging.Infrastructure.Contracts.Common.Enums;
using Messaging.Infrastructure.Contracts.QueueMessage.Commands;

namespace Messaging.API.Contracts.QueuedSingleMessage;

public record QueuedSingleMessageRequest(string Recipient, DeliveryMethodType DeliveryMethod, string Body)
{
    public static explicit operator EnqueueMessageCommand(QueuedSingleMessageRequest request)
    {
        return new EnqueueMessageCommand(new List<string>() { request.Recipient},request.DeliveryMethod,request.Body, "90009817");
    }
};

