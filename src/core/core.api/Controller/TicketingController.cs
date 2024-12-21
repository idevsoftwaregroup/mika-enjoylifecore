using core.application.Contract.API.DTO.Ticket;
using core.application.Contract.API.DTO.Ticket.Message;
using core.application.Contract.API.Interfaces;
using core.application.Exceptions;
using core.domain.entity.ticketingModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System.Globalization;
using common.defination.Utility;
using Newtonsoft.Json.Linq;
using System.Security.Claims;
using System.Net;
using core.application.Framework;
using core.domain.DomainModelDTOs.TicketingDTOs;
using Microsoft.AspNetCore.SignalR;
using core.api.Services;
using core.application.Services;
using System.Threading;
using core.domain.DomainModelDTOs.TicketingDTOs.FilterDTOs;

namespace core.api.Controller
{
    [CustomAuthorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TicketingController : ControllerBase
    {
        private readonly ITicketingService _ticketingService;
        private readonly IHubContext<HubHelper> _hubContext;

        public TicketingController(ITicketingService ticketingService, IHubContext<HubHelper> hubContext)
        {
            _ticketingService = ticketingService;
            _hubContext = hubContext;
        }

        [HttpPost("CreateTicket")]
        public async Task<ActionResult<OperationResult<Response_CommonOperationTicketDomainDTO>>> CreateTicket(CreateTicketRequestDTO requestDTO, CancellationToken cancellationToken = default)
        {

            var userId = Convert.ToInt32(HttpContext.User.FindFirst("CoreUserId")?.Value);
            var operation = await _ticketingService.CreateTicket(userId, requestDTO, cancellationToken);
            if (!operation.Success)
            {
                return StatusCode((int)operation.Status, operation);
            }
            await _hubContext.Clients.Clients(await _ticketingService.GetAllConnections()).SendAsync("ReloadTickets", cancellationToken: cancellationToken);
            return Ok(operation);

        }

        [HttpPut("ActivateTicket")]
        public async Task<ActionResult<OperationResult<Response_CommonOperationTicketDomainDTO>>> ActivateTicket(ActivateTicketRequestDTO requestDTO, CancellationToken cancellationToken = default)
        {
            var roleClaims = HttpContext.User.FindAll(ClaimTypes.Role);
            if (roleClaims == null || !roleClaims.Any() || !roleClaims.Select(x => Convert.ToString(x.Value)).ToList().Any(x => x.ToUpper() == "TICKET"))
            {
                return StatusCode((int)HttpStatusCode.Forbidden, new OperationResult<Response_CommonOperationTicketDomainDTO>("ActivateTicket").Failed("دسترسی فعالسازی درخواست برای کاربری شما وجود ندارد", HttpStatusCode.Forbidden));
            }
            var userId = Convert.ToInt32(HttpContext.User.FindFirst("CoreUserId")?.Value);
            var operation = await _ticketingService.ActivateTicket(userId, requestDTO, cancellationToken);
            if (!operation.Success)
            {
                return StatusCode((int)operation.Status, operation);
            }
            await _hubContext.Clients.Clients(await _ticketingService.GetAllConnections()).SendAsync("ReloadTickets", cancellationToken: cancellationToken);
            return Ok(operation);
        }

        [HttpPut("SucceedTicket")]
        public async Task<ActionResult<OperationResult<Response_CommonOperationTicketDomainDTO>>> SucceedTicket(ModifyTicketRequestDTO requestDTO, CancellationToken cancellationToken = default)
        {
            var roleClaims = HttpContext.User.FindAll(ClaimTypes.Role);
            if (roleClaims == null || !roleClaims.Any() || !roleClaims.Select(x => Convert.ToString(x.Value)).ToList().Any(x => x.ToUpper() == "TICKET"))
            {
                return StatusCode((int)HttpStatusCode.Forbidden, new OperationResult<Response_CommonOperationTicketDomainDTO>("SucceedTicket").Failed("دسترسی تغییر به انجام شده درخواست برای کاربری شما وجود ندارد", HttpStatusCode.Forbidden));
            }
            var userId = Convert.ToInt32(HttpContext.User.FindFirst("CoreUserId")?.Value);
            var operation = await _ticketingService.SucceedTicket(userId, requestDTO, cancellationToken);
            if (!operation.Success)
            {
                return StatusCode((int)operation.Status, operation);
            }
            await _hubContext.Clients.Clients(await _ticketingService.GetAllConnections()).SendAsync("ReloadTickets", cancellationToken: cancellationToken);
            return Ok(operation);
        }

        [HttpPut("CloseTicket")]
        public async Task<ActionResult<OperationResult<Response_CloseTicketDomainDTO>>> CloseTicket(ModifyTicketRequestDTO requestDTO, CancellationToken cancellationToken = default)
        {
            var userId = Convert.ToInt32(HttpContext.User.FindFirst("CoreUserId")?.Value);
            var operation = await _ticketingService.CloseTicket(userId, false, requestDTO, cancellationToken);
            if (!operation.Success)
            {
                return StatusCode((int)operation.Status, operation);
            }
            await _hubContext.Clients.Clients(await _ticketingService.GetAllConnections()).SendAsync("ReloadTickets", cancellationToken: cancellationToken);
            return Ok(operation);
        }

        [HttpPut("CloseTicketByAdmin")]
        public async Task<ActionResult<OperationResult<Response_CloseTicketDomainDTO>>> CloseTicketByAdmin(ModifyTicketRequestDTO requestDTO, CancellationToken cancellationToken = default)
        {
            var roleClaims = HttpContext.User.FindAll(ClaimTypes.Role);
            if (roleClaims == null || !roleClaims.Any() || !roleClaims.Select(x => Convert.ToString(x.Value)).ToList().Any(x => x.ToUpper() == "TICKET"))
            {
                return StatusCode((int)HttpStatusCode.Forbidden, new OperationResult<Response_CloseTicketDomainDTO>("CloseTicketByAdmin").Failed("دسترسی لغو درخواست برای کاربری شما وجود ندارد", HttpStatusCode.Forbidden));
            }
            var userId = Convert.ToInt32(HttpContext.User.FindFirst("CoreUserId")?.Value);
            var operation = await _ticketingService.CloseTicket(userId, true, requestDTO, cancellationToken);
            if (!operation.Success)
            {
                return StatusCode((int)operation.Status, operation);
            }
            await _hubContext.Clients.Clients(await _ticketingService.GetAllConnections()).SendAsync("ReloadTickets", cancellationToken: cancellationToken);
            return Ok(operation);
        }

        [HttpPut("UpdateTrackCode")]
        public async Task<ActionResult<OperationResult<Response_CommonOperationTicketDomainDTO>>> UpdateTrackCode(UpdateTrackingCodeDTO requestDTO, CancellationToken cancellationToken = default)
        {
            var roleClaims = HttpContext.User.FindAll(ClaimTypes.Role);
            if (roleClaims == null || !roleClaims.Any() || !roleClaims.Select(x => Convert.ToString(x.Value)).ToList().Any(x => x.ToUpper() == "TICKET"))
            {
                return StatusCode((int)HttpStatusCode.Forbidden, new OperationResult<Response_CommonOperationTicketDomainDTO>("UpdateTrackCode").Failed("دسترسی بروزرسانی درخواست برای کاربری شما وجود ندارد", HttpStatusCode.Forbidden));
            }
            var userId = Convert.ToInt32(HttpContext.User.FindFirst("CoreUserId")?.Value);
            var operation = await _ticketingService.UpdateTrackCode(userId, requestDTO, cancellationToken);
            if (!operation.Success)
            {
                return StatusCode((int)operation.Status, operation);
            }
            await _hubContext.Clients.Clients(await _ticketingService.GetAllConnections()).SendAsync("ReloadTickets", cancellationToken: cancellationToken);
            return Ok(operation);
        }

        [HttpPut("UpdateTicketNumber")]
        public async Task<ActionResult<OperationResult<Response_CommonOperationTicketDomainDTO>>> UpdateTicketNumber(UpdateTicketNumberDTO requestDTO, CancellationToken cancellationToken = default)
        {
            var roleClaims = HttpContext.User.FindAll(ClaimTypes.Role);
            if (roleClaims == null || !roleClaims.Any() || !roleClaims.Select(x => Convert.ToString(x.Value)).ToList().Any(x => x.ToUpper() == "TICKET"))
            {
                return StatusCode((int)HttpStatusCode.Forbidden, new OperationResult<Response_CommonOperationTicketDomainDTO>("UpdateTicketNumber").Failed("دسترسی بروزرسانی درخواست برای کاربری شما وجود ندارد", HttpStatusCode.Forbidden));
            }
            var userId = Convert.ToInt32(HttpContext.User.FindFirst("CoreUserId")?.Value);
            var operation = await _ticketingService.UpdateTicketNumber(userId, requestDTO, cancellationToken);
            if (!operation.Success)
            {
                return StatusCode((int)operation.Status, operation);
            }
            await _hubContext.Clients.Clients(await _ticketingService.GetAllConnections()).SendAsync("ReloadTickets", cancellationToken: cancellationToken);
            return Ok(operation);
        }

        [HttpPost("SeenTicket")]
        public async Task<ActionResult<OperationResult<Response_CommonOperationTicketDomainDTO>>> SeenTicket(SeenTicketDTO requestDTO, CancellationToken cancellationToken = default)
        {
            var roleClaims = HttpContext.User.FindAll(ClaimTypes.Role);
            if (roleClaims == null || !roleClaims.Any() || !roleClaims.Select(x => Convert.ToString(x.Value)).ToList().Any(x => x.ToUpper() == "TICKET"))
            {
                return StatusCode((int)HttpStatusCode.Forbidden, new OperationResult<Response_CommonOperationTicketDomainDTO>("SeenTicket").Failed("دسترسی تغییر به دیده شده درخواست برای کاربری شما وجود ندارد", HttpStatusCode.Forbidden));
            }
            var userId = Convert.ToInt32(HttpContext.User.FindFirst("CoreUserId")?.Value);
            var operation = await _ticketingService.SeenTicket(userId, requestDTO, cancellationToken);
            if (!operation.Success)
            {
                return StatusCode((int)operation.Status, operation);
            }
            await _hubContext.Clients.Clients(await _ticketingService.GetAllConnections()).SendAsync("ReloadTickets","SEEN", cancellationToken: cancellationToken);
            return Ok(operation);
        }

        [HttpPost("CreateMessage")]
        public async Task<ActionResult<OperationResult<Response_CreateTicketMessageDomainDTO>>> CreateMessage(CreateMessageRequestDTO requestDTO, CancellationToken cancellationToken = default)
        {
            var userId = Convert.ToInt32(HttpContext.User.FindFirst("CoreUserId")?.Value);
            var operation = await _ticketingService.CreateMessage(userId, false, requestDTO, cancellationToken);
            if (!operation.Success)
            {
                return StatusCode((int)operation.Status, operation);
            }
            await _hubContext.Clients.Clients(await _ticketingService.GetAllConnections()).SendAsync("ReloadTickets", cancellationToken: cancellationToken);
            return Ok(operation);
        }

        [HttpPost("CreateMessageByAdmin")]
        public async Task<ActionResult<OperationResult<Response_CreateTicketMessageDomainDTO>>> CreateMessageByAdmin(CreateMessageRequestDTO requestDTO, CancellationToken cancellationToken = default)
        {
            var roleClaims = HttpContext.User.FindAll(ClaimTypes.Role);
            if (roleClaims == null || !roleClaims.Any() || !roleClaims.Select(x => Convert.ToString(x.Value)).ToList().Any(x => x.ToUpper() == "TICKET"))
            {
                return StatusCode((int)HttpStatusCode.Forbidden, new OperationResult<Response_CreateTicketMessageDomainDTO>("CreateMessageByAdmin").Failed("دسترسی پاسخ به درخواست برای کاربری شما وجود ندارد", HttpStatusCode.Forbidden));
            }
            var userId = Convert.ToInt32(HttpContext.User.FindFirst("CoreUserId")?.Value);
            var operation = await _ticketingService.CreateMessage(userId, true, requestDTO, cancellationToken);
            if (!operation.Success)
            {
                return StatusCode((int)operation.Status, operation);
            }
            await _hubContext.Clients.Clients(await _ticketingService.GetAllConnections()).SendAsync("ReloadTickets", cancellationToken: cancellationToken);
            return Ok(operation);
        }

        [HttpGet("GetTickets")]
        public async Task<ActionResult<OperationResult<Response_GetTicketDomainDTO>>> GetTickets([FromQuery] Filter_GetTicketUserDomainDTO filter, CancellationToken cancellationToken = default)
        {
            var userId = Convert.ToInt32(HttpContext.User.FindFirst("CoreUserId")?.Value);
            var operation = await _ticketingService.GetTickets(userId, filter, cancellationToken: cancellationToken);
            if (!operation.Success)
            {
                return StatusCode((int)operation.Status, operation);
            }
            return Ok(operation);
        }

        [HttpGet("GetTicketsByAdmin")]
        public async Task<ActionResult<OperationResult<Response_GetTicketDomainDTO>>> GetTicketsByAdmin([FromQuery] Filter_GetTicketAdminDomainDTO filter, CancellationToken cancellationToken = default)
        {
            var roleClaims = HttpContext.User.FindAll(ClaimTypes.Role);
            if (roleClaims == null || !roleClaims.Any() || !roleClaims.Select(x => Convert.ToString(x.Value)).ToList().Any(x => x.ToUpper() == "TICKET"))
            {
                return StatusCode((int)HttpStatusCode.Forbidden, new OperationResult<Response_GetTicketDomainDTO>("GetTicketsByAdmin").Failed("دسترسی فراخوانی درخواست ها برای کاربری شما وجود ندارد", HttpStatusCode.Forbidden));
            }
            var userId = Convert.ToInt32(HttpContext.User.FindFirst("CoreUserId")?.Value);
            var operation = await _ticketingService.GetTicketsByAdmin(userId, filter, cancellationToken: cancellationToken);
            if (!operation.Success)
            {
                return StatusCode((int)operation.Status, operation);
            }
            return Ok(operation);
        }

        [HttpGet("GetTicketMessages/{TicketId}")]
        public async Task<ActionResult<OperationResult<Response_GetSingleTicketDomainDTO>>> GetTicketMessages(long TicketId, CancellationToken cancellationToken = default)
        {
            var userId = Convert.ToInt32(HttpContext.User.FindFirst("CoreUserId")?.Value);
            var operation = await _ticketingService.GetTicketMessages(userId, false, TicketId, cancellationToken);
            if (!operation.Success)
            {
                return StatusCode((int)operation.Status, operation);
            }
            return Ok(operation);
        }

        [HttpGet("GetTicketMessagesByAdmin/{TicketId}")]
        public async Task<ActionResult<OperationResult<Response_GetSingleTicketDomainDTO>>> GetTicketMessagesByAdmin(long TicketId, CancellationToken cancellationToken = default)
        {
            var roleClaims = HttpContext.User.FindAll(ClaimTypes.Role);
            if (roleClaims == null || !roleClaims.Any() || !roleClaims.Select(x => Convert.ToString(x.Value)).ToList().Any(x => x.ToUpper() == "TICKET"))
            {
                return StatusCode((int)HttpStatusCode.Forbidden, new OperationResult<Response_GetSingleTicketDomainDTO>("GetTicketMessagesByAdmin").Failed("دسترسی فراخوانی پاسخ های درخواست برای کاربری شما وجود ندارد", HttpStatusCode.Forbidden));
            }
            var userId = Convert.ToInt32(HttpContext.User.FindFirst("CoreUserId")?.Value);
            var operation = await _ticketingService.GetTicketMessages(userId, true, TicketId, cancellationToken);
            if (!operation.Success)
            {
                return StatusCode((int)operation.Status, operation);
            }
            return Ok(operation);
        }


        //[HttpGet("Tickets/All")]
        //public async Task<ActionResult<List<TicketPreviewResponseDTO>>> GetAllTickets([FromQuery] GetTicketListQueryParams queryParams, CancellationToken cancellationToken = default)
        //{
        //    var userId = Convert.ToInt32(HttpContext.User.FindFirst("CoreUserId")?.Value);
        //    return Ok(await _ticketingService.GetAllTickets(userId, queryParams, cancellationToken));
        //}

        //[HttpGet("Tickets")]
        //public async Task<ActionResult<List<TicketPreviewResponseDTO>>> GetTicketsByUser([FromQuery] GetTicketListQueryParams queryParams, CancellationToken cancellationToken = default)
        //{

        //    //var result  = await _ticketingService.GetTicketsByUserAndUnit(userId, unitId, queryParams, cancellationToken);
        //    var userId = Convert.ToInt32(HttpContext.User.FindFirst("CoreUserId")?.Value);
        //    var result = await _ticketingService.GetTicketsByUser(userId, queryParams, cancellationToken);
        //    return Ok(result);

        //}

        //[HttpGet("AllTickets")]
        //[CustomAuthorize("TICKET")]
        //public async Task<ActionResult<TicketsResponseDTO>> GetTickets([FromQuery] GetAdminTicketSearchDTO requestDTO, CancellationToken cancellationToken = default)
        //{
        //    var userId = Convert.ToInt32(HttpContext.User.FindFirst("CoreUserId")?.Value);
        //    var (tickets, totalCount) = await _ticketingService.GetTickets(userId, requestDTO, cancellationToken);
        //    return Ok(new TicketsResponseDTO
        //    {
        //        Tickets = tickets,
        //        TotalCount = totalCount
        //    });
        //}

        //[HttpGet("TicketsAdmin")]
        //public async Task<ActionResult<List<TicketPreviewResponseDTO>>> GetTicketsByUserAdmin(int userId, [FromQuery] GetTicketListQueryParams queryParams, CancellationToken cancellationToken = default)
        //{

        //    //var result  = await _ticketingService.GetTicketsByUserAndUnit(userId, unitId, queryParams, cancellationToken);
        //    //var userId = Convert.ToInt32(HttpContext.User.FindFirst("CoreUserId")?.Value);
        //    var result = await _ticketingService.GetTicketsByUser(userId, queryParams, cancellationToken);
        //    return Ok(result);

        //}
        //[HttpGet("Tickets/{ticketId}")]
        //public async Task<ActionResult<TicketDetailResponseDTO>> GetTicketDetail(int ticketId, CancellationToken cancellationToken = default, string? sortOrder = "desc")
        //{
        //    var userId = Convert.ToInt32(HttpContext.User.FindFirst("CoreUserId")?.Value);
        //    var data = await _ticketingService.GetTicketById(userId, ticketId, cancellationToken, sortOrder);
        //    return data != null ? Ok(data) : NoContent(); // add user id auth
        //}

        //[HttpGet("Tickets/{userId}/{ticketId}")]
        //[CustomAuthorize("TICKET")]
        //public async Task<ActionResult<TicketDetailResponseDTO>> GetTicketDetailAdmin(int userId, int ticketId, CancellationToken cancellationToken = default, string sortOrder = "desc")
        //{
        //    var data = await _ticketingService.GetTicketByIdAdmin(userId, ticketId, cancellationToken, sortOrder);

        //    return data != null ? Ok(data) : NoContent(); // add user id auth
        //}

        //[HttpPut("Tickets/{ticketId}/Admin")]
        //public async Task<ActionResult<TicketDetailResponseDTO>> UpdateTicketAdmin(int ticketId, UpdateTicketRequestDTO requestDTO, CancellationToken cancellationToken = default)
        //{
        //    return Ok(await _ticketingService.UpdateTicket(ticketId, true, requestDTO, cancellationToken));
        //}

        //[HttpPut("Tickets/{ticketId}")]
        //public async Task<ActionResult<TicketDetailResponseDTO>> UpdateTicket(int ticketId, UpdateTicketRequestDTO requestDTO, CancellationToken cancellationToken = default)
        //{

        //    requestDTO.VisitTime = null;
        //    requestDTO.TechnicianId = null;
        //    return Ok(await _ticketingService.UpdateTicket(ticketId, false, requestDTO, cancellationToken));
        //}

        //[HttpGet("TicketsVisitTimes")]
        //public async Task<ActionResult<List<TicketVisitTimeResponseDTO>?>> GetTicketsVisitTimes([FromQuery] GetTicketVisitSearchDTO requestDTO, CancellationToken cancellationToken = default)
        //{
        //    var userId = Convert.ToInt32(HttpContext.User.FindFirst("CoreUserId")?.Value);

        //    var result = await _ticketingService.GetTicketsVisitTimes(requestDTO, cancellationToken);

        //    if (result != null)
        //    {
        //        // Map each item in the result list to TicketVisitTimeResponseDTO
        //        var responseDTOs = result.Select(value => new TicketVisitTimeResponseDTO()
        //        {
        //            Id = value.Id,
        //            Ticket = value.Ticket,
        //            Order = value.Order,
        //            Title = value.Title,
        //            VisitTime = value.VisitTime.ToString("yyyy/MM/dd HH:mm:ss", new CultureInfo("fa-IR")),
        //        }).ToList();

        //        return responseDTOs;
        //    }

        //    return null;
        //}

        //[HttpPost("TicketVisitTime")]
        //[CustomAuthorize("TICKET")]
        //public async Task<ActionResult<int>> CreateTicketVisitTimeAdmin([FromBody] CreateTicketVisitTimeRequestDTO requestDTO,
        //                                                     int userId = 0,
        //                                                     CancellationToken cancellationToken = default)
        //{
        //    userId = Convert.ToInt32(HttpContext.User.FindFirst("CoreUserId")?.Value);
        //    //need to get operator role to give true isAdmin
        //    try
        //    {
        //        var result = await _ticketingService.CreateTicketVisitTime(requestDTO, cancellationToken);
        //        return Ok(result);
        //    }
        //    catch (WaitForOperatorException e)
        //    {
        //        return BadRequest(e.Message);
        //    }
        //}
        //[HttpDelete("TicketsVisitTimes/{visitId}")]
        //[CustomAuthorize("TICKET")]
        //public async Task<ActionResult<int>> DeleteVisitTime(int visitId, CancellationToken cancellationToken = default)
        //{
        //    return await _ticketingService.DeleteVisitTimeById(visitId, cancellationToken);
        //}
        //[HttpPut("SeenTicket")]
        //[CustomAuthorize("TICKET")]
        //public async Task<IActionResult> SeenTicket(SeenTicketDTO ticketIdDTO, CancellationToken cancellationToken = default)
        //{
        //    var roleClaims = HttpContext.User.FindAll(ClaimTypes.Role);
        //    var userId = HttpContext.User.FindFirst("CoreUserId")?.Value != null ? Convert.ToInt32(HttpContext.User.FindFirst("CoreUserId")?.Value) : 0;
        //    if (roleClaims == null || !roleClaims.Any() || userId <= 0)
        //    {
        //        return StatusCode((int)HttpStatusCode.Forbidden, "user dosen't have access");
        //    }
        //    if (!(roleClaims.Select(x => (Convert.ToString(x.Value)).ToUpper()).Any(x => x == "TICKET")))
        //    {
        //        return StatusCode((int)HttpStatusCode.Forbidden, "user dosen't have access");
        //    }
        //    var seenTicketOperation = await _ticketingService.SeenTicket(userId, ticketIdDTO, cancellationToken);
        //    return string.IsNullOrEmpty(seenTicketOperation) ? Ok($"seen ticket by id {ticketIdDTO.TicketId} successfully done!") : BadRequest(seenTicketOperation);
        //}
    }
}
