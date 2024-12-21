using core.application.Contract.API.DTO.Complex;
using core.application.Contract.API.DTO.EnjoyEvent;
using core.application.Contract.API.DTO.Party.Resident;
using core.application.Contract.API.Interfaces;
using core.application.Contract.infrastructure;
using core.application.Framework;
using core.application.Services;
using core.domain.entity.EnjoyEventModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace core.api.Controller
{
    [CustomAuthorize]
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly IEventService _EventService;

        public EventController(IEventService eventService)
        {
            _EventService = eventService;
        }
        [HttpGet("Events/{eventId}")]
        public async Task<ActionResult<GetEnjoyEventDetailResponseDTO>> GetEventDetail(int eventId, int unitId, CancellationToken cancellationToken = default)
        {
            var userId = Convert.ToInt32(HttpContext.User.FindFirst("CoreUserId")?.Value);
            return Ok(await _EventService.GetEnjoyEventDetail(eventId, unitId, userId, cancellationToken));
        }
        [HttpGet("GetEvents")]
        public async Task<ActionResult<List<EventTabDto>?>> GetEvents(int unitId, int tabId, CancellationToken cancellationToken = default)
        {
            return await _EventService.GetEvents(unitId, tabId, cancellationToken);
        }
        [HttpGet("GetSliderEvents")]
        public async Task<ActionResult<List<EventTabDto>?>> GetSliderEvents(int ComplexId, CancellationToken cancellationToken = default)
        {
            return await _EventService.GetSliderEvents(ComplexId, cancellationToken);

        }
        [HttpPost("CreateEvent")]
        public async Task<ActionResult> CreateEvent([FromBody] CreateEnjoyEventRequestDTO EventCreateRequestDTO)
        {
            int id = await _EventService.CreateEvent(EventCreateRequestDTO);
            return Ok(id);
        }
        [HttpPost("AddEventContent")]
        public async Task<ActionResult> AddEventContent([FromBody] CreateEventContentRequestDTO EventContentCreateRequestDTO)
        {
            int id = await _EventService.AddEventContent(EventContentCreateRequestDTO);
            return Ok(id);
        }
        [HttpPost("SetEventParty")]
        public async Task<ActionResult> SetEventParty([FromBody] SetEventPartyDTO SetEventPartyRequest)
        {
            long id = await _EventService.SetEventParty(SetEventPartyRequest);
            return Ok(id);
        }
        [HttpPost("AddEventSession")]
        public async Task<ActionResult> AddEventSession([FromBody] CreateSessionRequestDTO EventSessionCreateRequestDTO)
        {
            long id = await _EventService.AddEventSession(EventSessionCreateRequestDTO);
            return Ok(id);
        }
        [HttpPost("SaveEventTicket")]
        public async Task<ActionResult<OperationResult<TicketRequestDTO?>>> SaveEventTicket([FromBody] TicketRequestDTO ticketRequestDTO)
        {
            var userId = Convert.ToInt32(HttpContext.User.FindFirst("CoreUserId")?.Value);
            ticketRequestDTO.UserId = userId;
            var operation = await _EventService.SaveEventTicket(ticketRequestDTO);
            return !operation.Success ? StatusCode((int)operation.Status, operation) : Ok(operation);
        }
        [HttpPut("Event")]
        public async Task<ActionResult<EventModel>> UpdateEvent(int id, UpdateEnjoyEventDTO requestDTO, CancellationToken cancellationToken = default)
        {
            return Ok(await _EventService.UpdateEventAsync(id, requestDTO, cancellationToken));
        }
        [HttpPut("EventContent")]
        public async Task<ActionResult<EventContentModel>> UpdateEventContent(int eventId, int id, UpdateEventContentDTO requestDTO, CancellationToken cancellationToken = default)
        {
            return Ok(await _EventService.UpdateEventContent(eventId, id, requestDTO, cancellationToken));
        }
        [HttpPut("EventTicket")]
        public async Task<ActionResult<EventTicketModel>> UpdateEventTicket(long id, UpdateEventTicketRequestDTO requestDTO, CancellationToken cancellationToken = default)
        {
            return Ok(await _EventService.UpdateEventTicket(id, requestDTO, cancellationToken));
        }
        [HttpPut("EventMedia")]
        public async Task<ActionResult<EventMediaModel>> UpdateEventMediaModel(int id, UpdateEventMediaDTO requestDTO, CancellationToken cancellationToken = default)
        {
            return Ok(await _EventService.UpdateEventMedia(id, requestDTO, cancellationToken));
        }
        [HttpDelete("Event")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            await _EventService.DeleteEvent(id);
            return NoContent();
        }
        [HttpDelete("EventTicket")]
        public async Task<IActionResult> DeleteEventTicket(long id)
        {
            await _EventService.DeleteEventTicket(id);
            return NoContent();
        }
        [HttpDelete("EventSession")]
        public async Task<IActionResult> DeleteEventSession(int id)
        {
            await _EventService.DeleteEventSession(id);
            return NoContent();
        }
        [HttpDelete("EventMedia")]
        public async Task<IActionResult> DeleteEventMedia(int id)
        {
            await _EventService.DeleteEventMedia(id);
            return NoContent();
        }
    }
}
