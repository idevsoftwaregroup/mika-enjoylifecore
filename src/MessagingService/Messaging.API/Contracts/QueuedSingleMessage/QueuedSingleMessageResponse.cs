using Messaging.Infrastructure.Contracts.QueueMessage.Commands;

namespace Messaging.API.Contracts.QueuedSingleMessage;

public record QueuedSingleMessageResponse(bool isSuccessful, int MessageId, string? ResultMessage)
{
    public static implicit operator QueuedSingleMessageResponse(EnqueueMessageCommandResult result)
    {
        return new QueuedSingleMessageResponse(result.isSuccessful,result.MessageId,result.ResultMessage);
    }
};


