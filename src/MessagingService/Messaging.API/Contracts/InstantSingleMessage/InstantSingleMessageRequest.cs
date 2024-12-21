using Messaging.Infrastructure.Contracts.Common.Enums;
using Messaging.Infrastructure.Contracts.Message.Commands;

namespace Messaging.API.Contracts.InstantSingleMessage;

public record InstantSingleMessageRequest(string Recipient, DeliveryMethodType DeliveryMethod, string Body,string? Sender=null)
{
    public static explicit operator SendInstantMessageCommand(InstantSingleMessageRequest request)
    {
        return new SendInstantMessageCommand(request.Recipient, request.DeliveryMethod, request.Body,request.Sender);
    }
};

