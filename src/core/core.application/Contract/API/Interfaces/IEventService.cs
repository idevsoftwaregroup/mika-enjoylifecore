using core.application.Contract.API.DTO.EnjoyEvent;
using core.application.Framework;
using core.domain.entity.EnjoyEventModels;

using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace core.application.Contract.API.Interfaces;

public interface IEventService
{
    Task<int> CreateEvent(CreateEnjoyEventRequestDTO requestDTO, CancellationToken cancellationToken = default);         
    Task<int> AddEventContent(CreateEventContentRequestDTO requestDTO, CancellationToken cancellationToken = default);
    Task<int> AddEventSession(CreateSessionRequestDTO requestDTO, CancellationToken cancellationToken = default);
    Task<OperationResult<TicketRequestDTO?>> SaveEventTicket(TicketRequestDTO requestDTO, CancellationToken cancellationToken = default);
    //Task<List<EventTabDto>?> GetEvents(GetEventRequestDTO EventGetRequestDTO, CancellationToken cancellationToken = default);
    Task<EventModel> UpdateEventAsync(int id,UpdateEnjoyEventDTO updateEnjoyEventDTO, CancellationToken cancellationToken = default);
    Task<EventContentModel> UpdateEventContent(int eventId, int id, UpdateEventContentDTO updateEventContentDTO, CancellationToken cancellationToken = default);
    Task<EventSessionModel> UpdateEventSession(long id, UpdateSessionDTO updateSessionDTO, CancellationToken cancellationToken = default);
    Task<EventTicketModel> UpdateEventTicket(long id, UpdateEventTicketRequestDTO requestDTO, CancellationToken cancellationToken = default);
    Task<EventMediaModel> UpdateEventMedia(int id, UpdateEventMediaDTO updateEventMediaDTO, CancellationToken cancellationToken = default);
    Task DeleteEvent(int id);
    Task DeleteEventSession(int id);
    Task DeleteEventTicket(long id);
    Task DeleteEventMedia(int id);
    Task<List<EventTabDto>?> GetEvents(int unitId, int tabId, CancellationToken cancellationToken = default);
    Task<GetEnjoyEventDetailResponseDTO> GetEnjoyEventDetail(int eventId, int unitId, int userId, CancellationToken cancellationToken = default);
    Task<List<EventTabDto>?> GetSliderEvents(int ComplexId, CancellationToken cancellationToken = default);
    Task<int> SetEventParty(SetEventPartyDTO requestDTO);
}
