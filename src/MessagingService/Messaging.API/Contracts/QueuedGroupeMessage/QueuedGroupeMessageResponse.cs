using Messaging.Infrastructure.Contracts.QueueMessage.Commands;

namespace Messaging.API.Contracts.QueuedGroupeMessage;

public record QueuedGroupeMessageResponse(bool isSuccessful, int MessageId, string? ResultMessage)
{
    public static implicit operator QueuedGroupeMessageResponse(EnqueueMessageCommandResult result)
    {
        return new QueuedGroupeMessageResponse(result.isSuccessful, result.MessageId, result.ResultMessage);
    }
};


