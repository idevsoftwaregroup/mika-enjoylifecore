namespace Messaging.Infrastructure.Contracts.Message.Commands
{
    public record SendInstantMessageCommandResult(bool Sent, bool Delivered ,string? Message = null);
    
}
