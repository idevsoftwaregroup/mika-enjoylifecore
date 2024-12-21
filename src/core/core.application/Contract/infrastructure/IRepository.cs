using core.domain.entity;
using core.domain.entity.EnjoyEventModels;
using core.domain.entity.partyModels;
using core.domain.entity.structureModels;
using core.domain.entity.ticketingModels;
using Microsoft.EntityFrameworkCore;

namespace core.application.Contract.infrastructure;

public interface IRepository
{
    IQueryable<UserModel> Users { get; }
    IQueryable<UnitModel> Units { get; }
    //IQueryable<TicketModel> Tickets { get; }
    //IQueryable<TicketMessageModel> TicketMessages { get; }
    IQueryable<ResidentModel> Residents { get; }
    IQueryable<OwnerModel> Owners { get; }

    IQueryable<EventModel> Events { get; }
    IQueryable<EventSessionModel> EventSessions { get; }
    IQueryable<EventTicketModel> EventTickets { get; }
    IQueryable<ComplexModel> Complexes { get; }
    //IQueryable<TicketSeen> TicketSeens { get; }

}
