using core.application.Contract.API.DTO.EnjoyEvent;
using core.application.Contract.infrastructure;
using core.domain.entity.EnjoyEventModels;
using core.domain.entity.structureModels;
using core.infrastructure.Data.persist;
using core.infrastructure.Data.repository.exceptions;
using Microsoft.EntityFrameworkCore;

namespace core.infrastructure.Data.repository;

public class EventRepository : IEventRepository
{
    private readonly EnjoyLifeContext _context;

    public EventRepository(EnjoyLifeContext context)
    {
        _context = context;
    }    
    public async Task<EventModel?> GetEventAsync(int id)
    {
        return await _context.Events.Where(x => x.Id == id).Include(x=>x.EventContent).FirstOrDefaultAsync();
    }
    //public async Task<List<EventDto?>> GetMyEventAsync(int unitId)
    //{
    //    var currentTime = DateTime.Now;
    //    UnitModel unit = await _context.Units.FirstOrDefaultAsync(u => u.Id == unitId);
    //    BodyType thumbnailType = BodyType.THUMBNAIL;
    //    var comingSoon = await _context.Events
    //        .Where(e => e.ComplexId == unit.ComplexId)
    //        .Where(e => e.PublishDate <= currentTime && e.ReservationStartDate > currentTime)
    //        .Select(e => new EventDto
    //        {
    //            Id = e.Id,
    //            Title = e.Name,
    //            Date = null,
    //            Poster = e.EventContent
    //                .Where(c => c.BodyType == thumbnailType)
    //                .FirstOrDefault().Media
    //                .FirstOrDefault().Url,
    //            Status = 0
    //        })
    //        .ToListAsync();

    //    var onGoing = await _context.Events
    //        .Where(e => e.ComplexId == unit.ComplexId)
    //        .Where(e => e.PublishDate <= currentTime && e.ReservationStartDate <= currentTime)
    //        .Select(e => new EventDto
    //        {
    //            Id = e.Id,
    //            Title = e.Name,
    //            Date =  e.StartDate.ToString("d MMMM، yyyy"),
    //            Poster = e.EventContent
    //                .Where(c => c.BodyType == thumbnailType)
    //                .FirstOrDefault().Media
    //                .FirstOrDefault().Url,
    //            Status = 1
    //        })
    //        .ToListAsync();

    //    var result = comingSoon.Union(onGoing).ToList();
    //    return result;
    //}

    public async Task<List<EventModel>?> GetEventAsync(int unitId, int type)
    {
        var currentTime = DateTime.Now;
        UnitModel unit = await _context.Units.FirstOrDefaultAsync(u => u.Id == unitId);
        var events = _context.Events.Where(e => e.ComplexId == unit.ComplexId);
        switch (type)
        {
            case 0:
                events = events.Where(e => e.PublishDate <= currentTime && e.ReservationStartDate > currentTime).Include(c=>c.EventContent).ThenInclude(m=>m.Media);
                break;
            case 1:
                events = events.Where(e => e.PublishDate <= currentTime && e.ReservationStartDate <= currentTime && e.EndDate > currentTime
                && !e.EventSession.Any(t => t.Tickets.Any(t => t.Unit.Id == unitId))).Include(s=>s.EventSession).ThenInclude(t => t.Tickets).Include(c => c.EventContent).ThenInclude(m => m.Media);
                break;
            case 2:
                events = events.Where(e => e.PublishDate <= currentTime && e.EndDate < currentTime).Include(s => s.EventSession).ThenInclude(t => t.Tickets).Include(c => c.EventContent).ThenInclude(m => m.Media);
                break;
            case 3:
                events = events.Where(e => e.PublishDate <= currentTime && e.ReservationStartDate <= currentTime && e.EndDate > currentTime 
                && e.EventSession.Any(t => t.Tickets.Any(t => t.Unit.Id == unitId))).Include(s => s.EventSession).ThenInclude(t => t.Tickets).Include(c => c.EventContent).ThenInclude(m => m.Media);
                break;
        }

        var result = await events.ToListAsync();

        return result;
    }
    public async Task<List<EventModel>?> GetSliderEventsAsync(int ComplexId)
    { 
        var events = _context.Events.Where(e => e.ComplexId == ComplexId && e.IsPinned==true).Take(4)
            .OrderBy(o=>o.Id).Include(c => c.EventContent).ThenInclude(m => m.Media); 
        return await events.ToListAsync(); 
    }
    public async Task<EventSessionModel?> GetEventSessionAsync(long id)
    {
        return await _context.EventSessions.Where(x => x.Id == id).Include(e=>e.Event).Include(t=>t.Tickets).FirstOrDefaultAsync();
    }
    public async Task<EventTicketModel?> GetEventTicketAsync(long id)
    {
        return await _context.EventTickets.Where(x => x.Id == id).Include(x=>x.Session).Include(x=>x.Unit).FirstOrDefaultAsync();
    }
    public async Task<EventTicketModel?> GetEventTicketAsync(long sessionId, int unitId)
    {
        return await _context.EventTickets.Where(t=>t.Unit.Id==unitId && t.Session.Id==sessionId).FirstOrDefaultAsync();
    }
    public async Task<int> AddEnjoyEventAsync(EventModel enjoyEventModel, CancellationToken cancellationToken = default)
    {

        if (!_context.Complexes.Any(x => x.Id == enjoyEventModel.ComplexId))
        {
            throw new Exception("Complex doesnt exist");
        }
        await _context.Events.AddAsync(enjoyEventModel, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return enjoyEventModel.Id;
    }    
    public async Task<long> AddSessionAsync(EventSessionModel sessionModel, CancellationToken cancellationToken = default)
    {
        await _context.EventSessions.AddAsync(sessionModel, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return sessionModel.Id;
    }
    public async Task<EventTicketModel> AddReservationAsync(EventTicketModel reservationModel, CancellationToken cancellationToken = default)
    {
        await _context.EventTickets.AddAsync(reservationModel, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return reservationModel;
    }
    public async Task<int> AddContentAsync(EventContentModel eventContentModel, CancellationToken cancellationToken = default)
    {
        await _context.EventContents.AddAsync(eventContentModel, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return eventContentModel.Id;
    }
    public async Task<long> AddEventTicketAsync(EventTicketModel eventTicketModel, CancellationToken cancellationToken = default)
    {
        await _context.EventTickets.AddAsync(eventTicketModel, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return eventTicketModel.Id;
    }
    public async Task<EventMediaModel> AddMediaAsync(EventMediaModel eventMediaModel, CancellationToken cancellationToken = default)
    {
        await _context.EventMedia.AddAsync(eventMediaModel, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return eventMediaModel;
    }
    public async Task<int> UpdateEventAsync(EventModel Event)
    {
        if (await _context.Events.AnyAsync(x => x.Id == Event.Id))
        {
            _context.Events.Update(Event);
            return await SaveChangesAsync();
        }
        else return -1;
    }
    public async Task<int> UpdateEventTicketAsync(EventTicketModel eventTicket)
    {
        if (await _context.EventTickets.AnyAsync(x => x.Id == eventTicket.Id))
        {
            _context.EventTickets.Update(eventTicket);
            return await SaveChangesAsync();           
        }
        else return -1;
    }
    public async Task<EventPartyModel?> GetEventPartyAsync(int EventId, int UnitId)
    {
        return await _context.EventParty.Where(x => x.Event.Id == EventId && x.unit.Id==UnitId).FirstOrDefaultAsync();
    }
    public async Task<int> AddEventPartyAsync(EventPartyModel EventPartyModel)
    {
        await _context.EventParty.AddAsync(EventPartyModel);
        await _context.SaveChangesAsync();
        return EventPartyModel.Id;
    }
    public async Task<int> UpdateEventPartyAsync(EventPartyModel EventPartyModel)
    {
        if (await _context.EventParty.AnyAsync(x => x.Id == EventPartyModel.Id))
        {
            _context.EventParty.Update(EventPartyModel);
            return await SaveChangesAsync();
        }
        else return -1;
    }

    public async Task<bool?> DeleteEventTicketAsync(long id)
    {
        var ticketToDelete = await _context.EventTickets.FirstOrDefaultAsync(x => x.Id == id);
        if (ticketToDelete != null)
        {
            _context.EventTickets.Remove(ticketToDelete);
            await _context.SaveChangesAsync();
            return true;
        }
        else
        {
            return false;
        }
    }
    private async Task<int> SaveChangesAsync()
    {
        var now = DateTime.Now;

        foreach (var entry in _context.ChangeTracker.Entries<EventModel>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedDate = now;
                entry.Entity.ModifyDate = now;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.ModifyDate = now;
            }
        }

        return await _context.SaveChangesAsync();
    }

    public async Task UpdateSessionAsync(EventSessionModel session)
    {
        _context.EventSessions.Update(session);
        await _context.SaveChangesAsync();
    }

    public async Task<EventMediaModel> GetEventMediaAsync(int id)
    {
        return await _context.EventMedia.Where(m=>m.Id == id).FirstOrDefaultAsync();
        
    }

    public async Task UpdateMediaAsync(EventMediaModel media)
    {
        _context.EventMedia.Update(media);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteEventAsync(EventModel eventModel)
    {
        _context.Events.Remove(eventModel);
        await _context.SaveChangesAsync();
    }
    public async Task DeleteEventSessionAsync(EventSessionModel eventSessionModel)
    {
        _context.EventSessions.Remove(eventSessionModel);
        await _context.SaveChangesAsync();
    }
    public async Task DeleteEventTicketAsync(EventTicketModel eventTicketModel)
    {
        _context.EventTickets.Remove(eventTicketModel);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteEventMediaAsync(EventMediaModel media)
    {
        _context.EventMedia.Remove(media);
        await _context.SaveChangesAsync();
    }

}
