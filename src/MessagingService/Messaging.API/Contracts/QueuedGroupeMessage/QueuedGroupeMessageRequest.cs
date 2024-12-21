using Messaging.Infrastructure.Contracts.Common.Enums;
using Messaging.Infrastructure.Contracts.QueueMessage.Commands;

namespace Messaging.API.Contracts.QueuedGroupeMessage;

public record QueuedGroupeMessageRequest(IEnumerable<string> Recipients, DeliveryMethodType DeliveryMethod, string Body)
{
    public static explicit operator EnqueueMessageCommand(QueuedGroupeMessageRequest request)
    {
        return new EnqueueMessageCommand(request.Recipients, request.DeliveryMethod, request.Body,"90009817");
    }
};

