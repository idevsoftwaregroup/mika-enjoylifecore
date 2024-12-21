namespace Messaging.Infrastructure.Contracts.QueueMessage.Commands;

public record DequeueMessageCommand(ushort numberOfMessagesDequeued,TimeSpan listentingTime);

