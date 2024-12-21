using Messaging.Infrastructure.Contracts.Message.Commands;

namespace Messaging.API.Contracts.InstantSingleMessage;

public record InstantSingleMessageResponse(bool Sent, bool Delivered, string? Message)
{
    public static implicit operator InstantSingleMessageResponse(SendInstantMessageCommandResult result)
    {
        return new InstantSingleMessageResponse(result.Sent, result.Delivered, result.Message);
    }
};

