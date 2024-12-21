using core.application.Contract.infrastructure;
using core.domain.entity;
using core.domain.entity.EnjoyEventModels;
using core.domain.entity.partyModels;
using core.domain.entity.structureModels;
using core.domain.entity.ticketingModels;
using core.infrastructure.Data.persist;
using Microsoft.EntityFrameworkCore;

namespace core.infrastructure.Data.repository;

public class Repository : IRepository
{
    private readonly EnjoyLifeContext _context;

    public Repository(EnjoyLifeContext context)
    {
        _context = context;
    }

    public IQueryable<UserModel> Users => _context.Users.AsNoTracking();

    public IQueryable<UnitModel> Units => _context.Units.AsNoTracking();

    //public IQueryable<TicketModel> Tickets => _context.Tickets.AsNoTracking();

    //public IQueryable<TicketMessageModel> TicketMessages => _context.TicketMessages.AsNoTracking();

    public IQueryable<ResidentModel> Residents => _context.Residents.AsNoTracking();

    public IQueryable<OwnerModel> Owners => _context.Owners.AsNoTracking();

    public IQueryable<EventModel> Events => _context.Events.AsNoTracking();
    public IQueryable<EventSessionModel> EventSessions => _context.EventSessions.AsNoTracking();
    public IQueryable<EventTicketModel> EventTickets => _context.EventTickets.AsNoTracking();
    public IQueryable<ComplexModel> Complexes => _context.Complexes.AsNoTracking();
    //public IQueryable<TicketSeen> TicketSeens => _context.TicketSeens.AsNoTracking();

    //public async Task RemoveMultiMedia(MultiMediaModel model, CancellationToken cancellationToken = default)
    //{
    //    _context.MultiMedias.Remove(model);
    //    await _context.SaveChangesAsync(cancellationToken);
    //}

    //public async Task<MultiMediaModel> UpdateMultiMedia(MultiMediaModel model, CancellationToken cancellationToken = default)
    //{
    //    _context.MultiMedias.Update(model);
    //    await _context.SaveChangesAsync(cancellationToken);

    //    return model;
    //}

    //public async Task<MultiMediaModel> AddMultiMedia(MultiMediaModel model, CancellationToken cancellationToken = default)
    //{
    //    _context.MultiMedias.Add(model);
    //    await _context.SaveChangesAsync(cancellationToken);

    //    return model;
    //}

    //public async Task<MultiMediaModel?> GetMultiMedia(long id, CancellationToken cancellationToken = default)
    //{
    //    return await _context.MultiMedias.Where(m => m.Id == id).SingleOrDefaultAsync();
    //}
}
