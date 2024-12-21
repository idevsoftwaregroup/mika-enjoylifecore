using Messaging.Infrastructure.Domain;

namespace Messaging.Infrastructure.Services.Persistence
{
    public interface IMessagingRepository
    {
        Task<Message> AddMessageAsync(Message message, CancellationToken cancellationToken);
        Task<Message?> GetMessageAsync(int MessageId, CancellationToken cancellationToken);
        Task<Message> UpdateMessage(Message message, CancellationToken cancellationToken);
        Task<List<BulkInstanceMessage>> AddRangeBulkInstanceMessageAsync(List<BulkInstanceMessage> models, CancellationToken cancellationToken = default);
        Task<List<BulkInstanceMessage>> UpdateRangeBulkInstanceMessageAsync(List<BulkInstanceMessage> models, CancellationToken cancellationToken = default);
        Task<List<BulkInstanceMessage>> GetNotSentBulkMessages(CancellationToken cancellationToken = default);
    }
}