using core.application.Contract.API.DTO.Reservation.Joint;
using core.application.Contract.API.DTO.Reservation.Joint.Filters;
using core.application.Contract.API.DTO.Reservation.Reservation;
using core.application.Contract.API.DTO.Reservation.Reservation.Filter;
using core.application.Contract.API.DTO.Reservation.Reservation.ServiceDTOs;
using core.application.Contract.API.DTO.Reservation.Ticket;
using core.application.Contract.API.Interfaces;
using core.application.Framework;
using core.domain.DomainModelDTOs.TicketingDTOs;
using core.domain.entity.enums;
using core.domain.entity.ReservationModels.JointModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace core.api.Controller
{
    [CustomAuthorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationController : ControllerBase
    {
        private readonly IReservationService _reservationService;
        private readonly HttpClient _httpClient;
        public ReservationController(IReservationService reservationService, HttpClient httpClient)
        {
            this._reservationService = reservationService;
            this._httpClient = httpClient;
        }

        [HttpGet("GetComplexes")]
        public async Task<ActionResult<OperationResult<Response_GetComplexDTO>>> GetComplexes(CancellationToken cancellationToken = default)
        {
            var operation = await _reservationService.GetComplexes(cancellationToken);
            return operation.Success ? Ok(operation) : StatusCode((int)operation.Status, operation);
        }

        [HttpGet("GetUnits")]
        public async Task<ActionResult<OperationResult<Response_GetUnitDTO>>> GetUnits(CancellationToken cancellationToken = default)
        {
            var operation = await _reservationService.GetUnits(cancellationToken);
            return operation.Success ? Ok(operation) : StatusCode((int)operation.Status, operation);
        }

        [HttpGet("GetUnitsByUserId/{CustomerId}")]
        public async Task<ActionResult<OperationResult<Response_GetUnitDTO>>> GetUnitsByUserId(int CustomerId, CancellationToken cancellationToken = default)
        {
            var operation = await _reservationService.GetUnitsByUserId(CustomerId, cancellationToken);
            return operation.Success ? Ok(operation) : StatusCode((int)operation.Status, operation);
        }

        [HttpGet("GetCustomers")]
        public async Task<ActionResult<OperationResult<Middle_GetReservationsByAdmin_ReservedBy>>> GetCustomers([FromQuery] int? UnitId, CancellationToken cancellationToken = default)
        {
            var roleClaims = HttpContext.User.FindAll(ClaimTypes.Role);
            if (roleClaims == null || !roleClaims.Any() || !roleClaims.Select(x => Convert.ToString(x.Value)).ToList().Any(x => x.ToUpper() == "ADMIN"))
            {
                return StatusCode((int)HttpStatusCode.Forbidden, new OperationResult<object>("GetCustomers").Failed("دسترسی عملیات برای کاربری شما وجود ندارد", HttpStatusCode.Forbidden));
            }
            var operation = await _reservationService.GetCustomers(UnitId, cancellationToken);
            return operation.Success ? Ok(operation) : StatusCode((int)operation.Status, operation);
        }

        [HttpGet("GetJointStatuses")]
        public async Task<ActionResult<OperationResult<Response_GetJointStatusDTO>>> GetJointStatuses(CancellationToken cancellationToken = default)
        {
            var operation = await _reservationService.GetJointStatuses(cancellationToken);
            return operation.Success ? Ok(operation) : StatusCode((int)operation.Status, operation);
        }

        [HttpGet("GetJoints")]
        public async Task<ActionResult<OperationResult<Response_GetJointDTO>>> GetJoints([FromQuery] Filter_GetJointDTO? filter, CancellationToken cancellationToken = default)
        {
            var operation = await _reservationService.GetJoints(filter, cancellationToken);
            return operation.Success ? Ok(operation) : StatusCode((int)operation.Status, operation);
        }

        [HttpGet("GetJoint/{JointId}")]
        public async Task<ActionResult<OperationResult<Response_GetJointDTO>>> GetJoint(int JointId, CancellationToken cancellationToken = default)
        {
            var operation = await _reservationService.GetJoint(JointId, cancellationToken);
            return operation.Success ? Ok(operation) : StatusCode((int)operation.Status, operation);
        }

        [HttpPost("CreateJoint")]
        public async Task<ActionResult<OperationResult<object>>> CreateJoint(Request_CreateJointDTO model, CancellationToken cancellationToken = default)
        {
            var roleClaims = HttpContext.User.FindAll(ClaimTypes.Role);
            if (roleClaims == null || !roleClaims.Any() || !roleClaims.Select(x => Convert.ToString(x.Value)).ToList().Any(x => x.ToUpper() == "ADMIN"))
            {
                return StatusCode((int)HttpStatusCode.Forbidden, new OperationResult<object>("CreateJoint").Failed("دسترسی عملیات برای کاربری شما وجود ندارد", HttpStatusCode.Forbidden));
            }
            var operation = await _reservationService.CreateJoint(model, cancellationToken);
            return operation.Success ? Ok(operation) : StatusCode((int)operation.Status, operation);
        }

        [HttpPut("UpdateJoint")]
        public async Task<ActionResult<OperationResult<object>>> UpdateJoint(Request_UpdateJointDTO model, CancellationToken cancellationToken = default)
        {
            var roleClaims = HttpContext.User.FindAll(ClaimTypes.Role);
            if (roleClaims == null || !roleClaims.Any() || !roleClaims.Select(x => Convert.ToString(x.Value)).ToList().Any(x => x.ToUpper() == "ADMIN"))
            {
                return StatusCode((int)HttpStatusCode.Forbidden, new OperationResult<object>("UpdateJoint").Failed("دسترسی عملیات برای کاربری شما وجود ندارد", HttpStatusCode.Forbidden));
            }
            var operation = await _reservationService.UpdateJoint(model, cancellationToken);
            return operation.Success ? Ok(operation) : StatusCode((int)operation.Status, operation);
        }

        [HttpDelete("DeleteJoint")]
        public async Task<ActionResult<OperationResult<object>>> DeleteJoint(Request_DeleteJointDTO model, CancellationToken cancellationToken = default)
        {
            var roleClaims = HttpContext.User.FindAll(ClaimTypes.Role);
            if (roleClaims == null || !roleClaims.Any() || !roleClaims.Select(x => Convert.ToString(x.Value)).ToList().Any(x => x.ToUpper() == "ADMIN"))
            {
                return StatusCode((int)HttpStatusCode.Forbidden, new OperationResult<object>("DeleteJoint").Failed("دسترسی عملیات برای کاربری شما وجود ندارد", HttpStatusCode.Forbidden));
            }
            var operation = await _reservationService.DeleteJoint(model, cancellationToken);
            return operation.Success ? Ok(operation) : StatusCode((int)operation.Status, operation);
        }

        [HttpGet("GetGenders")]
        public ActionResult<OperationResult<Response_GetGenderDTO>> GetGenders()
        {
            OperationResult<Response_GetGenderDTO> Op = new("GetGenders");
            return Ok(Op.Succeed("دریافت لیست جنسیت ها با موفقیت انجام شد", new List<Response_GetGenderDTO>
            {
                new() {
                    Id = (int)GenderType.FAMALE,
                    Title = "Female",
                    DisplayTitle = "بانوان"
                },
                new() {
                    Id = (int)GenderType.MALE,
                    Title = "Male",
                    DisplayTitle = "آقایان"
                },
                new() {
                    Id = (int)GenderType.ALL,
                    Title = "All",
                    DisplayTitle = "همگانی"
                }
            }));
        }

        [HttpGet("GetJointSessionsByAdmin")]
        public async Task<ActionResult<OperationResult<Response_GetJointSessionByAdminDTO>>> GetJointSessionsByAdmin([FromQuery] Filter_GetJointSessionByAdminDTO? filter, CancellationToken cancellationToken = default)
        {
            var roleClaims = HttpContext.User.FindAll(ClaimTypes.Role);
            if (roleClaims == null || !roleClaims.Any() || !roleClaims.Select(x => Convert.ToString(x.Value)).ToList().Any(x => x.ToUpper() == "ADMIN"))
            {
                return StatusCode((int)HttpStatusCode.Forbidden, new OperationResult<object>("GetJointSessionsByAdmin").Failed("دسترسی عملیات برای کاربری شما وجود ندارد", HttpStatusCode.Forbidden));
            }
            var operation = await _reservationService.GetJointSessionsByAdmin(filter, cancellationToken);
            return operation.Success ? Ok(operation) : StatusCode((int)operation.Status, operation);
        }

        [HttpGet("GetJointSessionsByCustomer/{CustomerId}/{UnitId}/{JointId}/{RequestDate}")]
        public async Task<ActionResult<OperationResult<Response_GetJointSessionByCustomerDTO>>> GetJointSessionsByCustomer(int CustomerId, int UnitId, int JointId, DateTime RequestDate, [FromQuery] Filter_GetJointSessionByCustomerDTO? filter, CancellationToken cancellationToken = default)
        {
            var operation = await _reservationService.GetJointSessionsByCustomer(CustomerId, UnitId, JointId, RequestDate, filter, cancellationToken);
            return operation.Success ? Ok(operation) : StatusCode((int)operation.Status, operation);
        }

        [HttpGet("GetSingleJointSessionByCustomer/{CustomerId}/{UnitId}/{JointSessionId}")]
        public async Task<ActionResult<OperationResult<Middle_GetJointSessionsByCustomerDTO>>> GetSingleJointSessionByCustomer(int CustomerId, int UnitId, long JointSessionId, CancellationToken cancellationToken = default)
        {
            var operation = await _reservationService.GetSingleJointSessionByCustomer(CustomerId, UnitId, JointSessionId, cancellationToken);
            return operation.Success ? Ok(operation) : StatusCode((int)operation.Status, operation);
        }

        [HttpGet("GetYearsMonths")]
        public ActionResult<OperationResult<Response_GetYearMonthDTO>> GetYearsMonths()
        {
            var roleClaims = HttpContext.User.FindAll(ClaimTypes.Role);
            if (roleClaims == null || !roleClaims.Any() || !roleClaims.Select(x => Convert.ToString(x.Value)).ToList().Any(x => x.ToUpper() == "ADMIN"))
            {
                return StatusCode((int)HttpStatusCode.Forbidden, new OperationResult<object>("GetYearsMonths").Failed("دسترسی عملیات برای کاربری شما وجود ندارد", HttpStatusCode.Forbidden));
            }
            var operation = _reservationService.GetYearsMonths();
            return operation.Success ? Ok(operation) : StatusCode((int)operation.Status, operation);
        }

        [HttpPost("CreateJointSessions")]
        public async Task<ActionResult<OperationResult<object>>> CreateJointSessions(Request_CreateJointSessionsServiceDTO model, CancellationToken cancellationToken = default)
        {
            var roleClaims = HttpContext.User.FindAll(ClaimTypes.Role);
            if (roleClaims == null || !roleClaims.Any() || !roleClaims.Select(x => Convert.ToString(x.Value)).ToList().Any(x => x.ToUpper() == "ADMIN"))
            {
                return StatusCode((int)HttpStatusCode.Forbidden, new OperationResult<object>("CreateJointSessions").Failed("دسترسی عملیات برای کاربری شما وجود ندارد", HttpStatusCode.Forbidden));
            }
            var operation = await _reservationService.CreateJointSessions(Convert.ToInt32(HttpContext.User.FindFirst("CoreUserId")?.Value), model, cancellationToken);
            return operation.Success ? Ok(operation) : StatusCode((int)operation.Status, operation);

        }

        [HttpPost("CreateSingleJointSession")]
        public async Task<ActionResult<OperationResult<object>>> CreateSingleJointSession(Request_CreateSingleJointSessionServiceDTO model, CancellationToken cancellationToken = default)
        {
            var roleClaims = HttpContext.User.FindAll(ClaimTypes.Role);
            if (roleClaims == null || !roleClaims.Any() || !roleClaims.Select(x => Convert.ToString(x.Value)).ToList().Any(x => x.ToUpper() == "ADMIN"))
            {
                return StatusCode((int)HttpStatusCode.Forbidden, new OperationResult<object>("CreateSingleJointSession").Failed("دسترسی عملیات برای کاربری شما وجود ندارد", HttpStatusCode.Forbidden));
            }
            var operation = await _reservationService.CreateSingleJointSession(Convert.ToInt32(HttpContext.User.FindFirst("CoreUserId")?.Value), model, cancellationToken);
            return operation.Success ? Ok(operation) : StatusCode((int)operation.Status, operation);
        }

        [HttpPut("UpdateClosureJointSession")]
        public async Task<ActionResult<OperationResult<object>>> UpdateClosureJointSession(Request_UpdateClosureJointSessionServiceDTO model, CancellationToken cancellationToken = default)
        {
            var roleClaims = HttpContext.User.FindAll(ClaimTypes.Role);
            if (roleClaims == null || !roleClaims.Any() || !roleClaims.Select(x => Convert.ToString(x.Value)).ToList().Any(x => x.ToUpper() == "ADMIN"))
            {
                return StatusCode((int)HttpStatusCode.Forbidden, new OperationResult<object>("UpdateClosureJointSession").Failed("دسترسی عملیات برای کاربری شما وجود ندارد", HttpStatusCode.Forbidden));
            }
            var operation = await _reservationService.UpdateClosureJointSession(Convert.ToInt32(HttpContext.User.FindFirst("CoreUserId")?.Value), model, cancellationToken);
            return operation.Success ? Ok(operation) : StatusCode((int)operation.Status, operation);
        }

        [HttpPut("UpdatePrivateJointSession")]
        public async Task<ActionResult<OperationResult<object>>> UpdatePrivateJointSession(Request_UpdatePrivateJointSessionServiceDTO model, CancellationToken cancellationToken = default)
        {
            var roleClaims = HttpContext.User.FindAll(ClaimTypes.Role);
            if (roleClaims == null || !roleClaims.Any() || !roleClaims.Select(x => Convert.ToString(x.Value)).ToList().Any(x => x.ToUpper() == "ADMIN"))
            {
                return StatusCode((int)HttpStatusCode.Forbidden, new OperationResult<object>("UpdatePrivateJointSession").Failed("دسترسی عملیات برای کاربری شما وجود ندارد", HttpStatusCode.Forbidden));
            }
            var operation = await _reservationService.UpdatePrivateJointSession(Convert.ToInt32(HttpContext.User.FindFirst("CoreUserId")?.Value), model, cancellationToken);
            return operation.Success ? Ok(operation) : StatusCode((int)operation.Status, operation);
        }

        [HttpPut("UpdatePublicJointSession")]
        public async Task<ActionResult<OperationResult<object>>> UpdatePublicJointSession(Request_UpdatePublicJointSessionServiceDTO model, CancellationToken cancellationToken = default)
        {
            var roleClaims = HttpContext.User.FindAll(ClaimTypes.Role);
            if (roleClaims == null || !roleClaims.Any() || !roleClaims.Select(x => Convert.ToString(x.Value)).ToList().Any(x => x.ToUpper() == "ADMIN"))
            {
                return StatusCode((int)HttpStatusCode.Forbidden, new OperationResult<object>("UpdatePublicJointSession").Failed("دسترسی عملیات برای کاربری شما وجود ندارد", HttpStatusCode.Forbidden));
            }
            var operation = await _reservationService.UpdatePublicJointSession(Convert.ToInt32(HttpContext.User.FindFirst("CoreUserId")?.Value), model, cancellationToken);
            return operation.Success ? Ok(operation) : StatusCode((int)operation.Status, operation);
        }

        [HttpDelete("DeleteJointSession")]
        public async Task<ActionResult<OperationResult<object>>> DeleteJointSession(Request_DeleteJointSessionDTO model, CancellationToken cancellationToken = default)
        {
            var roleClaims = HttpContext.User.FindAll(ClaimTypes.Role);
            if (roleClaims == null || !roleClaims.Any() || !roleClaims.Select(x => Convert.ToString(x.Value)).ToList().Any(x => x.ToUpper() == "ADMIN"))
            {
                return StatusCode((int)HttpStatusCode.Forbidden, new OperationResult<object>("DeleteJointSession").Failed("دسترسی عملیات برای کاربری شما وجود ندارد", HttpStatusCode.Forbidden));
            }
            var operation = await _reservationService.DeleteJointSession(model, cancellationToken);
            return operation.Success ? Ok(operation) : StatusCode((int)operation.Status, operation);
        }

        [HttpGet("GetReservationReportByAdmin/{JointId}")]
        public async Task<ActionResult<OperationResult<Response_GetReservationReportDTO>>> GetReservationReportByAdmin(int JointId, [FromQuery] Filter_GetReservationReportDTO? filter, CancellationToken cancellationToken = default)
        {
            var roleClaims = HttpContext.User.FindAll(ClaimTypes.Role);
            if (roleClaims == null || !roleClaims.Any() || !roleClaims.Select(x => Convert.ToString(x.Value)).ToList().Any(x => x.ToUpper() == "ADMIN"))
            {
                return StatusCode((int)HttpStatusCode.Forbidden, new OperationResult<object>("GetReservationReportByAdmin").Failed("دسترسی عملیات برای کاربری شما وجود ندارد", HttpStatusCode.Forbidden));
            }
            var operation = await _reservationService.GetReservationReportByAdmin(JointId, filter, cancellationToken);
            return operation.Success ? Ok(operation) : StatusCode((int)operation.Status, operation);
        }

        [HttpGet("GetReservationsByAdmin/{JointId}")]
        public async Task<ActionResult<OperationResult<Response_GetReservationsByAdminDTO>>> GetReservationsByAdmin(int JointId, [FromQuery] Filter_GetReservationsByAdminDTO? filter, CancellationToken cancellationToken = default)
        {
            var roleClaims = HttpContext.User.FindAll(ClaimTypes.Role);
            if (roleClaims == null || !roleClaims.Any() || !roleClaims.Select(x => Convert.ToString(x.Value)).ToList().Any(x => x.ToUpper() == "ADMIN"))
            {
                return StatusCode((int)HttpStatusCode.Forbidden, new OperationResult<object>("GetReservationsByAdmin").Failed("دسترسی عملیات برای کاربری شما وجود ندارد", HttpStatusCode.Forbidden));
            }
            var operation = await _reservationService.GetReservationsByAdmin(JointId, filter, cancellationToken);
            return operation.Success ? Ok(operation) : StatusCode((int)operation.Status, operation);
        }

        [HttpGet("GetReservationsByCustomer")]
        public async Task<ActionResult<OperationResult<Response_GetReservationsByCustomerDTO>>> GetReservationsByCustomer([FromQuery] Filter_GetReservationsByCustomerDTO? filter, CancellationToken cancellationToken = default)
        {
            var operation = await _reservationService.GetReservationsByCustomer(Convert.ToInt32(HttpContext.User.FindFirst("CoreUserId")?.Value), filter, cancellationToken);
            return operation.Success ? Ok(operation) : StatusCode((int)operation.Status, operation);
        }

        [HttpPost("ReserveJointSessionByCustomer")]
        public async Task<ActionResult<OperationResult<object>>> ReserveJointSessionByCustomer(Request_ReserveJointSessionServiceDTO model, CancellationToken cancellationToken = default)
        {
            var operation = await _reservationService.ReserveJointSessionByCustomer(model, cancellationToken);
            return operation.Success ? Ok(operation) : StatusCode((int)operation.Status, operation);
        }

        [HttpPut("CancelReservationByAdmin")]
        public async Task<ActionResult<OperationResult<object>>> CancelReservationByAdmin(Request_CancelReservationDTO model, CancellationToken cancellationToken = default)
        {
            var roleClaims = HttpContext.User.FindAll(ClaimTypes.Role);
            if (roleClaims == null || !roleClaims.Any() || !roleClaims.Select(x => Convert.ToString(x.Value)).ToList().Any(x => x.ToUpper() == "ADMIN"))
            {
                return StatusCode((int)HttpStatusCode.Forbidden, new OperationResult<object>("CancelReservationByAdmin").Failed("دسترسی عملیات برای کاربری شما وجود ندارد", HttpStatusCode.Forbidden));
            }
            var operation = await _reservationService.CancelReservationByAdmin(Convert.ToInt32(HttpContext.User.FindFirst("CoreUserId")?.Value), model, cancellationToken);
            if (!operation.Success)
            {
                StatusCode((int)operation.Status, new OperationResult<object>("CancelReservationByAdmin").Failed(operation.Message, operation.ExMessage, operation.Status));
            }
            var text = $"{operation.Object.ReservedByGenderDisplayTitle} {operation.Object.ReservedByFullName} عزیز\nرزرو {operation.Object.JonitTitle}\nبرای سانس {operation.Object.SessionDate.GetPersianDateString().ToPersianNumber()}، {operation.Object.StartTime.GetHhMmFromTimeSpan().ToPersianNumber()} - {operation.Object.EndTime.GetHhMmFromTimeSpan().ToPersianNumber()}\nبعلت : {operation.Object.CancellationDescription}\nدر تاریخ {operation.OperationDate.GetPersianDateString().ToPersianNumber()} ساعت {operation.OperationDate.GetHhMmFromDate().ToPersianNumber()} توسط ادمین لغو گردید.";
            await _httpClient.GetAsync($"https://api.kavenegar.com/v1/69334E5866502B443547553667716F32794E5974376D58465A6A4C53646C54324E68682F2F5348497779493D/sms/send.json?receptor={operation.Object.TargetPhoneNumber}&sender=90009817&message={text}", cancellationToken);
            return Ok(new OperationResult<object>("CancelReservationByAdmin").Succeed(operation.Message, operation.Status));
        }

        [HttpPut("CancelReservationByCustomer")]
        public async Task<ActionResult<OperationResult<object>>> CancelReservationByCustomer(Request_CancelReservationDTO model, CancellationToken cancellationToken = default)
        {
            var operation = await _reservationService.CancelReservationByCustomer(Convert.ToInt32(HttpContext.User.FindFirst("CoreUserId")?.Value), model, cancellationToken);
            if (!operation.Success)
            {
                StatusCode((int)operation.Status, new OperationResult<object>("CancelReservationByCustomer").Failed(operation.Message, operation.ExMessage, operation.Status));
            }
            if (operation.Object.TargetPhoneNumber.IsNotNull())
            {
                var text = $"ادمین محترم\nرزرو {operation.Object.JonitTitle}\nسانس {operation.Object.SessionDate.GetPersianDateString().ToPersianNumber()}، {operation.Object.StartTime.GetHhMmFromTimeSpan().ToPersianNumber()} - {operation.Object.EndTime.GetHhMmFromTimeSpan().ToPersianNumber()}\nتوسط {operation.Object.ReservedByGenderDisplayTitle} {operation.Object.ReservedByFullName}\nدر تاریخ {operation.OperationDate.GetPersianDateString().ToPersianNumber()} ساعت {operation.OperationDate.GetHhMmFromDate().ToPersianNumber()}\n.لغو گردید";
                await _httpClient.GetAsync($"https://api.kavenegar.com/v1/69334E5866502B443547553667716F32794E5974376D58465A6A4C53646C54324E68682F2F5348497779493D/sms/send.json?receptor={operation.Object.TargetPhoneNumber}&sender=90009817&message={text}", cancellationToken);
            }
            return Ok(new OperationResult<object>("CancelReservationByCustomer").Succeed(operation.Message, operation.Status));

        }
    }
}
