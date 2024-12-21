using Messaging.Infrastructure.Domain;
using Microsoft.EntityFrameworkCore;

namespace Messaging.Infrastructure.Services.Persistence;

public class MessagingRepository : IMessagingRepository
{
    private readonly MessagingDBContext _context;

    public MessagingRepository(MessagingDBContext context)
    {
        _context = context;
    }

    public async Task<Message> AddMessageAsync(Message message, CancellationToken cancellationToken)
    {
        await _context.Messages.AddAsync(message, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return message;
    }

    public async Task<List<BulkInstanceMessage>> AddRangeBulkInstanceMessageAsync(List<BulkInstanceMessage> models, CancellationToken cancellationToken = default)
    {
        await _context.BulkInstances.AddRangeAsync(models, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return models;
    }

    public async Task<Message?> GetMessageAsync(int MessageId, CancellationToken cancellationToken)
    {
        return await _context.Messages.SingleOrDefaultAsync(x => x.Id == MessageId, cancellationToken);
    }

    public async Task<Message> UpdateMessage(Message message, CancellationToken cancellationToken)
    {
        _context.Update(message);
        await _context.SaveChangesAsync(cancellationToken);
        return message;
    }

    public async Task<List<BulkInstanceMessage>> UpdateRangeBulkInstanceMessageAsync(List<BulkInstanceMessage> models, CancellationToken cancellationToken = default)
    {
        _context.BulkInstances.UpdateRange(models);
        await _context.SaveChangesAsync(cancellationToken);

        return models;
    }

    public async Task<List<BulkInstanceMessage>> GetNotSentBulkMessages(CancellationToken cancellationToken = default)
    {
        return await _context.BulkInstances.Where(m => !m.Sent).ToListAsync(cancellationToken);
    }
}
