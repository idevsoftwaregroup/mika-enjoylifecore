using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mika.Application.Services.Interfaces;
using Mika.Domain.Contracts.DTOs.Users;
using Mika.Domain.Contracts.DTOs.Users.Filter;
using Mika.Framework.Models;
using Mika.Framework.Models.Pagination;
namespace Mika.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserApplication _userApplication;
        public UserController(IUserApplication userApplication)
        {
            this._userApplication = userApplication;
        }
        [HttpGet("GetUsers")]
        public async Task<ActionResult<OperationResult<PagedList<Respone_GetUserDTO>>>> GetUsers([FromQuery] Filter_GetUserDTO? filter, [FromQuery] PageModel? page, CancellationToken cancellationToken = default)
        {
            var UserId = Convert.ToInt64(HttpContext.User.Claims.First(x => x.Type == "UserId")?.Value);
            var operation = await _userApplication.GetUsers(UserId, filter, page, cancellationToken);
            return operation.Success ? Ok(operation) : StatusCode((int)operation.Status, operation);
        }

        [HttpGet("GetUser/{UserId}")]
        public async Task<ActionResult<OperationResult<Respone_GetUserDTO>>> GetUser(long UserId, CancellationToken cancellationToken = default)
        {
            var ContextUserId = Convert.ToInt64(HttpContext.User.Claims.First(x => x.Type == "UserId")?.Value);
            var operation = await _userApplication.GetUser(ContextUserId, new Request_UserIdDTO
            {
                UserId = UserId,
            }, cancellationToken);
            return operation.Success ? Ok(operation) : StatusCode((int)operation.Status, operation);
        }

        [HttpPost("CreateAdmin")]
        public async Task<ActionResult<OperationResult<object>>> CreateAdmin(Request_CreateAdminDTO model, CancellationToken cancellationToken = default)
        {
            var SuperAdminId = Convert.ToInt64(HttpContext.User.Claims.First(x => x.Type == "UserId")?.Value);
            var operation = await _userApplication.CreateAdmin(SuperAdminId, model, cancellationToken);
            return operation.Success ? Ok(operation) : StatusCode((int)operation.Status, operation);
        }

        [HttpPut("UpdateAdmin")]
        public async Task<ActionResult<OperationResult<object>>> UpdateAdmin(Request_UpdateAdminDTO model, CancellationToken cancellationToken = default)
        {
            var SuperAdminId = Convert.ToInt64(HttpContext.User.Claims.First(x => x.Type == "UserId")?.Value);
            var operation = await _userApplication.UpdateAdmin(SuperAdminId, model, cancellationToken);
            return operation.Success ? Ok(operation) : StatusCode((int)operation.Status, operation);
        }

        [HttpPost("CreateUser")]
        public async Task<ActionResult<OperationResult<object>>> CreateUser(Request_CreateUserDTO model, CancellationToken cancellationToken = default)
        {
            var CreatorId = Convert.ToInt64(HttpContext.User.Claims.First(x => x.Type == "UserId")?.Value);
            var operation = await _userApplication.CreateUser(CreatorId, model, cancellationToken);
            return operation.Success ? Ok(operation) : StatusCode((int)operation.Status, operation);
        }

        [HttpPut("UpdateUser")]
        public async Task<ActionResult<OperationResult<object>>> UpdateUser(Request_UpdateUserDTO model, CancellationToken cancellationToken = default)
        {
            var ModifierId = Convert.ToInt64(HttpContext.User.Claims.First(x => x.Type == "UserId")?.Value);
            var operation = await _userApplication.UpdateUser(ModifierId, model, cancellationToken);
            return operation.Success ? Ok(operation) : StatusCode((int)operation.Status, operation);
        }

    }
}
