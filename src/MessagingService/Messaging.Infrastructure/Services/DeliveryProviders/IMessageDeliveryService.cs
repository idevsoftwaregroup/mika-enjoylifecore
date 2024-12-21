using Messaging.Infrastructure.Contracts.InstantMessage.Commands;
using Messaging.Infrastructure.Contracts.Message.Commands;

namespace Messaging.Infrastructure.Services.DeliveryProviders
{
    public interface IMessageDeliveryService
    {
        Task<SendInstantMessageCommandResult> SendInstantMessageAsync(SendInstantMessageCommand command, CancellationToken cancellationToken);
        Task<SendInstantMessageCommandResult> DeliverQueuedMessageAsync(SendInstantMessageCommand command, CancellationToken cancellationToken);
        Task<SendBulkMessageResult> SendBulkMessageAsync(SendBulkMessageCommand command, CancellationToken cancellationToken);
        Task<SendBulkMessageResult> RetrySendBulkMessage(RetryBulkMessageCommand command, CancellationToken cancellationToken);
    }
}
