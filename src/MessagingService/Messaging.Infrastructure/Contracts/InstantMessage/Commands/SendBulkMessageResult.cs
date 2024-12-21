namespace Messaging.Infrastructure.Contracts.InstantMessage.Commands;

public class SendBulkMessageResult
{
    public Dictionary<string,bool> Status { get; set; } = new Dictionary<string,bool>();
}
