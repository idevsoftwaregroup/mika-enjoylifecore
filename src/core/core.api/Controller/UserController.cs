using core.application.Contract.API.DTO.Party.User;
using core.application.Contract.API.Interfaces;
using core.application.Framework;
using core.domain.DomainModelDTOs.UserDTOs;
using DocumentFormat.OpenXml.Spreadsheet;
using Irony.Parsing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Claims;

namespace core.web.api.Controllers
{
    [CustomAuthorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: api/User/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserGetResponseDTO>> GetUser(int id)
        {
            var user = await _userService.GetUserById(id);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpGet("GetUsersByUnitId/{UnitId}")]
        public async Task<ActionResult<OperationResult<Response_GetUsersByUnitDomainDTO>>> GetUsersByUnitId(int UnitId, CancellationToken cancellationToken = default)
        {
            var roleClaims = HttpContext.User.FindAll(ClaimTypes.Role);
            if (roleClaims == null || !roleClaims.Any() || !roleClaims.Select(x => Convert.ToString(x.Value)).ToList().Any(x => x.ToUpper() == "ADMIN" || x.ToUpper() == "FINANCE" || x.ToUpper() == "TICKET" || x.ToUpper() == "EVENT"))
            {
                return StatusCode((int)HttpStatusCode.Forbidden, new OperationResult<object>("GetUsersByUnitId").Failed("دسترسی دریافت اطلاعات کاربران برای کاربری شما وجود ندارد", HttpStatusCode.Forbidden));
            }
            var operation = await _userService.GetUsersByUnitId(new Request_UnitIdDomainDTO
            {
                UnitId = UnitId,
            }, cancellationToken);
            return !operation.Success ? StatusCode((int)operation.Status, operation) : Ok(operation);
        }

        [HttpGet]
        [Route("GetAllUser")]
        [CustomAuthorize("FINANCE")]
        public async Task<ActionResult<UserGetResponseDTO>> GetAllUser()
        {
            var user = await _userService.GetAllUser();

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }
        [HttpGet]
        [Route("GetUserByPhone")]
        public async Task<ActionResult<UserGetResponseDTO>> GetUserByPhone(string phoneNumber)
        {
            var user = await _userService.GetUserByPhone(phoneNumber);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpGet]
        [Route("GetUsersPhoneNumber")]
        [AllowAnonymous]
        public async Task<ActionResult<List<string>>> GetUsersPhoneNumberAdmin(int userId, int ComplexId)
        {
            //check if user is admin
            var PhoneNumbers = await _userService.GetUsersPhoneNumber(ComplexId);

            if (PhoneNumbers == null)
            {
                return NotFound();
            }

            return Ok(PhoneNumbers);
        }


        //GetUsersPhoneNumber

        [HttpPost("Createuser")]
        public async Task<ActionResult<UserGetResponseDTO>> Createuser([FromBody] UserCreateRequest userCreateDTO)
        {

            int id = await _userService.CreateUser(userCreateDTO);
            return Ok(id);
        }

        // PUT: api/User/5
        [HttpPut("UpdateUser")]
        public async Task<IActionResult> UpdateUser([FromBody] UserUpdateRequest userUpdateDTO)
        {
            if (userUpdateDTO is null || userUpdateDTO.Id <= 0)
            {
                return BadRequest();
            }

            bool updated = await _userService.UpdateUser(userUpdateDTO);

            if (!updated)
            {
                return NotFound();
            }

            return Ok(updated);
        }

        [HttpPost("CreateUserAndResident")]
        public async Task<ActionResult<OperationResult<object>>> CreateUserAndResident(Request_CreateUserAndResidentDomainDTO model, CancellationToken cancellationToken = default)
        {
            var token = (Request.Headers["Authorization"]).ToString().Replace("Bearer ", string.Empty).Replace("bearer ", string.Empty);
            var roleClaims = HttpContext.User.FindAll(ClaimTypes.Role);
            if (roleClaims == null || !roleClaims.Any() || !roleClaims.Select(x => Convert.ToString(x.Value)).ToList().Any(x => x.ToUpper() == "ADMIN"))
            {
                return StatusCode((int)HttpStatusCode.Forbidden, new OperationResult<object>("CreateUserAndResident").Failed("دسترسی ساخت ساکن جدید برای کاربری شما وجود ندارد", HttpStatusCode.Forbidden));
            }
            var userId = Convert.ToInt32(HttpContext.User.FindFirst("CoreUserId")?.Value);
            var operation = await _userService.CreateUserAndResident(token, userId, model, cancellationToken);

            return !operation.Success ? StatusCode((int)operation.Status, operation) : Ok(operation);
        }

        [HttpPost("CreateUserAndOwner")]
        public async Task<ActionResult<OperationResult<object>>> CreateUserAndOwner(Request_CreateUserAndOwnerDomainDTO model, CancellationToken cancellationToken = default)
        {
            var token = (Request.Headers["Authorization"]).ToString().Replace("Bearer ", string.Empty).Replace("bearer ", string.Empty);
            var roleClaims = HttpContext.User.FindAll(ClaimTypes.Role);
            if (roleClaims == null || !roleClaims.Any() || !roleClaims.Select(x => Convert.ToString(x.Value)).ToList().Any(x => x.ToUpper() == "ADMIN"))
            {
                return StatusCode((int)HttpStatusCode.Forbidden, new OperationResult<object>("CreateUserAndOwner").Failed("دسترسی ساخت مالک جدید برای کاربری شما وجود ندارد", HttpStatusCode.Forbidden));
            }
            var userId = Convert.ToInt32(HttpContext.User.FindFirst("CoreUserId")?.Value);
            var operation = await _userService.CreateUserAndOwner(token, userId, model, cancellationToken);

            return !operation.Success ? StatusCode((int)operation.Status, operation) : Ok(operation);
        }

        [HttpPut("UpdateUserAndResident")]
        public async Task<ActionResult<OperationResult<object>>> UpdateUserAndResident(Request_UpdateUserAndResidentDomainDTO model, CancellationToken cancellationToken = default)
        {
            var token = (Request.Headers["Authorization"]).ToString().Replace("Bearer ", string.Empty).Replace("bearer ", string.Empty);
            var roleClaims = HttpContext.User.FindAll(ClaimTypes.Role);
            if (roleClaims == null || !roleClaims.Any() || !roleClaims.Select(x => Convert.ToString(x.Value)).ToList().Any(x => x.ToUpper() == "ADMIN"))
            {
                return StatusCode((int)HttpStatusCode.Forbidden, new OperationResult<object>("UpdateUserAndResident").Failed("دسترسی بروزرسانی اطلاعات ساکن برای کاربری شما وجود ندارد", HttpStatusCode.Forbidden));
            }
            var userId = Convert.ToInt32(HttpContext.User.FindFirst("CoreUserId")?.Value);
            var operation = await _userService.UpdateUserAndResident(token, userId, model, cancellationToken);

            return !operation.Success ? StatusCode((int)operation.Status, operation) : Ok(operation);
        }

        [HttpPut("UpdateUserAndOwner")]
        public async Task<ActionResult<OperationResult<object>>> UpdateUserAndOwner(Request_UpdateUserAndOwnerDomainDTO model, CancellationToken cancellationToken = default)
        {
            var token = (Request.Headers["Authorization"]).ToString().Replace("Bearer ", string.Empty).Replace("bearer ", string.Empty);
            var roleClaims = HttpContext.User.FindAll(ClaimTypes.Role);
            if (roleClaims == null || !roleClaims.Any() || !roleClaims.Select(x => Convert.ToString(x.Value)).ToList().Any(x => x.ToUpper() == "ADMIN"))
            {
                return StatusCode((int)HttpStatusCode.Forbidden, new OperationResult<object>("UpdateUserAndOwner").Failed("دسترسی بروزرسانی اطلاعات مالک برای کاربری شما وجود ندارد", HttpStatusCode.Forbidden));
            }
            var userId = Convert.ToInt32(HttpContext.User.FindFirst("CoreUserId")?.Value);
            var operation = await _userService.UpdateUserAndOwner(token, userId, model, cancellationToken);

            return !operation.Success ? StatusCode((int)operation.Status, operation) : Ok(operation);
        }

        [HttpDelete("DeleteUserAndOwnerResident")]
        public async Task<ActionResult<OperationResult<object>>> DeleteUserAndOwnerResident(Request_DeleteUserDomainDTO model, CancellationToken cancellationToken = default)
        {
            var token = (Request.Headers["Authorization"]).ToString().Replace("Bearer ", string.Empty).Replace("bearer ", string.Empty);
            var roleClaims = HttpContext.User.FindAll(ClaimTypes.Role);
            if (roleClaims == null || !roleClaims.Any() || !roleClaims.Select(x => Convert.ToString(x.Value)).ToList().Any(x => x.ToUpper() == "ADMIN"))
            {
                return StatusCode((int)HttpStatusCode.Forbidden, new OperationResult<object>("DeleteUserAndOwnerResident").Failed("دسترسی حذف ساکن یا مالک برای کاربری شما وجود ندارد", HttpStatusCode.Forbidden));
            }
            var userId = Convert.ToInt32(HttpContext.User.FindFirst("CoreUserId")?.Value);
            var operation = await _userService.DeleteUserAndOwnerResident(token, userId, model, cancellationToken);

            return !operation.Success ? StatusCode((int)operation.Status, operation) : Ok(operation);
        }

        [HttpDelete("DeleteOwnership")]
        public async Task<ActionResult<OperationResult<object>>> DeleteOwnership(Request_DeleteUserDomainDTO model, CancellationToken cancellationToken = default)
        {
            var roleClaims = HttpContext.User.FindAll(ClaimTypes.Role);
            if (roleClaims == null || !roleClaims.Any() || !roleClaims.Select(x => Convert.ToString(x.Value)).ToList().Any(x => x.ToUpper() == "ADMIN"))
            {
                return StatusCode((int)HttpStatusCode.Forbidden, new OperationResult<object>("DeleteOwnership").Failed("دسترسی حذف مالکیت برای کاربری شما وجود ندارد", HttpStatusCode.Forbidden));
            }
            var operation = await _userService.DeleteOwnership(model, cancellationToken);

            return !operation.Success ? StatusCode((int)operation.Status, operation) : Ok(operation);
        }

        [HttpDelete("DeleteResidence")]
        public async Task<ActionResult<OperationResult<object>>> DeleteResidence(Request_DeleteUserDomainDTO model, CancellationToken cancellationToken = default)
        {
            var roleClaims = HttpContext.User.FindAll(ClaimTypes.Role);
            if (roleClaims == null || !roleClaims.Any() || !roleClaims.Select(x => Convert.ToString(x.Value)).ToList().Any(x => x.ToUpper() == "ADMIN"))
            {
                return StatusCode((int)HttpStatusCode.Forbidden, new OperationResult<object>("DeleteResidence").Failed("دسترسی حذف سکونت برای کاربری شما وجود ندارد", HttpStatusCode.Forbidden));
            }
            var operation = await _userService.DeleteResidence(model, cancellationToken);

            return !operation.Success ? StatusCode((int)operation.Status, operation) : Ok(operation);
        }

        [HttpPost("CreateResidentsOwnersOfUnit")]
        public async Task<ActionResult<OperationResult<object>>> CreateResidentsOwnersOfUnit(Request_CreateResidentsOwnersOfUnitDomainDTO model, CancellationToken cancellationToken = default)
        {
            var token = (Request.Headers["Authorization"]).ToString().Replace("Bearer ", string.Empty).Replace("bearer ", string.Empty);
            var roleClaims = HttpContext.User.FindAll(ClaimTypes.Role);
            if (roleClaims == null || !roleClaims.Any() || !roleClaims.Select(x => Convert.ToString(x.Value)).ToList().Any(x => x.ToUpper() == "ADMIN"))
            {
                return StatusCode((int)HttpStatusCode.Forbidden, new OperationResult<object>("CreateUserAndResident").Failed("دسترسی ساخت ساکن جدید برای کاربری شما وجود ندارد", HttpStatusCode.Forbidden));
            }
            var userId = Convert.ToInt32(HttpContext.User.FindFirst("CoreUserId")?.Value);
            var operation = await _userService.CreateResidentsOwnersOfUnit(token, userId, model, cancellationToken);

            return !operation.Success ? StatusCode((int)operation.Status, operation) : Ok(operation);
        }

        [HttpGet("GetCoreUsers")]
        public async Task<ActionResult<OperationResult<Response_GetUserDomainDTO>>> GetCoreUsers([FromQuery] Filter_GetUserDomainDTO? filter, CancellationToken cancellationToken = default)
        {
            var roleClaims = HttpContext.User.FindAll(ClaimTypes.Role);
            if (roleClaims == null || !roleClaims.Any() || !roleClaims.Select(x => Convert.ToString(x.Value)).ToList().Any(x => x.ToUpper() == "ADMIN"))
            {
                return StatusCode((int)HttpStatusCode.Forbidden, new OperationResult<Response_GetUserDomainDTO>("GetCoreUsers").Failed("دسترسی کاربری شما وجود ندارد", HttpStatusCode.Forbidden));
            }
            var operation = await _userService.GetCoreUsers(filter, cancellationToken);
            return !operation.Success ? StatusCode((int)operation.Status, operation) : Ok(operation);
        }

        [HttpPost("CreateCoreUser")]
        public async Task<ActionResult<OperationResult<Response_CreateOrModifiedUserIdDomainDTO>>> CreateCoreUser(Request_CreateUserDomainDTO model, CancellationToken cancellationToken = default)
        {
            var roleClaims = HttpContext.User.FindAll(ClaimTypes.Role);
            if (roleClaims == null || !roleClaims.Any() || !roleClaims.Select(x => Convert.ToString(x.Value)).ToList().Any(x => x.ToUpper() == "ADMIN"))
            {
                return StatusCode((int)HttpStatusCode.Forbidden, new OperationResult<Response_CreateOrModifiedUserIdDomainDTO>("CreateCoreUser").Failed("دسترسی کاربری شما وجود ندارد", HttpStatusCode.Forbidden));
            }
            var userId = Convert.ToInt32(HttpContext.User.FindFirst("CoreUserId")?.Value);
            var operation = await _userService.CreateCoreUser(userId, model, cancellationToken);
            return !operation.Success ? StatusCode((int)operation.Status, operation) : Ok(operation);
        }

        [HttpPut("UpdateCoreUser")]
        public async Task<ActionResult<OperationResult<Response_CreateOrModifiedUserIdDomainDTO>>> UpdateCoreUser(Request_UpdateUserDomainDTO model, CancellationToken cancellationToken = default)
        {
            var roleClaims = HttpContext.User.FindAll(ClaimTypes.Role);
            if (roleClaims == null || !roleClaims.Any() || !roleClaims.Select(x => Convert.ToString(x.Value)).ToList().Any(x => x.ToUpper() == "ADMIN"))
            {
                return StatusCode((int)HttpStatusCode.Forbidden, new OperationResult<Response_CreateOrModifiedUserIdDomainDTO>("UpdateCoreUser").Failed("دسترسی کاربری شما وجود ندارد", HttpStatusCode.Forbidden));
            }
            var userId = Convert.ToInt32(HttpContext.User.FindFirst("CoreUserId")?.Value);
            var operation = await _userService.UpdateCoreUser(userId, model, cancellationToken);
            return !operation.Success ? StatusCode((int)operation.Status, operation) : Ok(operation);
        }

        [HttpPost("DeleteCoreUser")]
        public async Task<ActionResult<OperationResult<object>>> DeleteCoreUser(Request_DeleteUserDomainDTO model, CancellationToken cancellationToken = default)
        {
            var roleClaims = HttpContext.User.FindAll(ClaimTypes.Role);
            if (roleClaims == null || !roleClaims.Any() || !roleClaims.Select(x => Convert.ToString(x.Value)).ToList().Any(x => x.ToUpper() == "ADMIN"))
            {
                return StatusCode((int)HttpStatusCode.Forbidden, new OperationResult<Response_CreateOrModifiedUserIdDomainDTO>("DeleteCoreUser").Failed("دسترسی کاربری شما وجود ندارد", HttpStatusCode.Forbidden));
            }
            var userId = Convert.ToInt32(HttpContext.User.FindFirst("CoreUserId")?.Value);
            var operation = await _userService.DeleteCoreUser(userId, model, cancellationToken);
            return !operation.Success ? StatusCode((int)operation.Status, operation) : Ok(operation);
        }


    }
}
