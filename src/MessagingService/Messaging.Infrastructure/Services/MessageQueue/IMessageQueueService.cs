using Messaging.Infrastructure.Contracts.QueueMessage.Commands;

namespace Messaging.Infrastructure.Services.MessageQueue
{
    public interface IMessageQueueService
    {
        Task<EnqueueMessageCommandResult> EnqueueMessageAsync(EnqueueMessageCommand command, CancellationToken cancellationToken);
        Task<DequeueMessageCommandResult> DequeueMessageAsync(DequeueMessageCommand command, CancellationToken cancellationToken);
    }
}
