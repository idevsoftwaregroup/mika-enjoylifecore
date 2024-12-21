namespace Messaging.Infrastructure.Contracts.QueueMessage.Commands;

public record EnqueueMessageCommandResult(bool isSuccessful, int MessageId, string? ResultMessage);

