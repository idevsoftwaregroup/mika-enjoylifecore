using core.application.Contract.API.DTO.EnjoyEvent;
using core.domain.entity.EnjoyEventModels;

namespace core.application.Contract.infrastructure
{
    public interface IEventRepository
    {
        Task<EventModel?> GetEventAsync(int id);
        //Task<List<EventDto?>> GetMyEventAsync(int unitId);
        Task<List<EventModel>?> GetEventAsync(int unitId, int type);
        Task<List<EventModel>?> GetSliderEventsAsync(int ComplexId);
        Task<EventSessionModel?> GetEventSessionAsync(long id);
        Task<EventTicketModel?> GetEventTicketAsync(long id);
        Task<EventTicketModel?> GetEventTicketAsync(long sessionId, int unitId);
        Task<int> AddEnjoyEventAsync(EventModel enjoyEventModel, CancellationToken cancellationToken = default);
        Task<EventTicketModel> AddReservationAsync(EventTicketModel reservationModel, CancellationToken cancellationToken = default);
        Task<long> AddSessionAsync(EventSessionModel sessionModel, CancellationToken cancellationToken = default);        
        Task<int> AddContentAsync(EventContentModel eventContentModel, CancellationToken cancellationToken = default);
        Task<long> AddEventTicketAsync(EventTicketModel eventTicketModel, CancellationToken cancellationToken = default);
        Task<EventMediaModel> AddMediaAsync(EventMediaModel eventMediaModel, CancellationToken cancellationToken = default);
        Task<int> UpdateEventAsync(EventModel Event);
        Task<int> UpdateEventTicketAsync(EventTicketModel eventTicket);
        Task<EventPartyModel?> GetEventPartyAsync(int EventId, int UnitId);
        Task<int> AddEventPartyAsync(EventPartyModel EventPartyModel);
        Task<int> UpdateEventPartyAsync(EventPartyModel EventPartyModel);
        Task<bool?> DeleteEventTicketAsync(long id);
        Task UpdateSessionAsync(EventSessionModel session);
        Task<EventMediaModel> GetEventMediaAsync(int id);
        Task UpdateMediaAsync(EventMediaModel media);
        Task DeleteEventAsync(EventModel eventModel);
        Task DeleteEventSessionAsync(EventSessionModel eventSessionModel);
        Task DeleteEventTicketAsync(EventTicketModel eventTicketModel);
        Task DeleteEventMediaAsync(EventMediaModel media);
    }
}