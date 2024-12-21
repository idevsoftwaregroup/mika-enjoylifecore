using core.application.Contract.API.Interfaces;
using core.domain.DomainModelDTOs.LogDTOs.ActionDTOs.FilterDTOs;
using core.domain.DomainModelDTOs.LogDTOs.ActionDTOs;
using core.domain.DomainModelDTOs.LogDTOs.ActivityDTOs.FilterDTOs;
using core.domain.DomainModelDTOs.LogDTOs.ActivityDTOs;
using core.domain.DomainModelDTOs.LogDTOs.ModuleDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;
using core.application.Framework;

namespace core.api.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    [CustomAuthorize]
    public class LogController : ControllerBase
    {
        private readonly IActivityService _activityService;
        private readonly IActionService _actionService;
        public LogController(IActivityService activityService, IActionService actionService)
        {
            _activityService = activityService;
            _actionService = actionService;
        }
        [AllowAnonymous]
        [HttpGet("GetModules")]
        public async Task<ActionResult<OperationResult<GetModuleDTO>>> GetModules(CancellationToken cancellationToken = default)
        {
            var operation = await _actionService.GetModules(cancellationToken);
            return operation.Success ? Ok(operation) : StatusCode((int)operation.Status, operation);
        }
        [HttpGet("GetActivityLogs")]
        public async Task<ActionResult<OperationResult<GetActivityLogDTO>>> GetActivityLogs([FromQuery] Filter_GetActivityLogDTO filter, CancellationToken cancellationToken = default)
        {
            var roleClaims = HttpContext.User.FindAll(ClaimTypes.Role);
            if (roleClaims == null || !roleClaims.Any() || !roleClaims.Select(x => Convert.ToString(x.Value)).ToList().Any(x => x.ToUpper() == "ADMIN"))
            {
                return StatusCode((int)HttpStatusCode.Forbidden, new OperationResult<GetActivityLogDTO>("GetActivityLogs").Failed("دسترسی دریافت لیست لاگ ها برای کاربری شما وجود ندارد", HttpStatusCode.Forbidden));
            }
            var operation = await _activityService.GetActivityLogs(filter, cancellationToken);
            return operation.Success ? Ok(operation) : StatusCode((int)operation.Status, operation);

        }
        [HttpGet("GetActionLogs")]
        public async Task<ActionResult<OperationResult<GetActionLogDTO>>> GetActionLogs([FromQuery] Filter_GetActionLogDTO filter, CancellationToken cancellationToken = default)
        {
            var roleClaims = HttpContext.User.FindAll(ClaimTypes.Role);
            if (roleClaims == null || !roleClaims.Any() || !roleClaims.Select(x => Convert.ToString(x.Value)).ToList().Any(x => x.ToUpper() == "ADMIN"))
            {
                return StatusCode((int)HttpStatusCode.Forbidden, new OperationResult<GetActionLogDTO>("GetActionLogs").Failed("دسترسی دریافت لیست لاگ ها برای کاربری شما وجود ندارد", HttpStatusCode.Forbidden));
            }
            var operation = await _actionService.GetActionLogs(filter, cancellationToken);
            return operation.Success ? Ok(operation) : StatusCode((int)operation.Status, operation);
        }
        [HttpPost("SetLog_Login")]
        public async Task<ActionResult<OperationResult<object>>> SetLog_Login(CancellationToken cancellationToken = default)
        {
            var userId = Convert.ToInt32(HttpContext.User.FindFirst("CoreUserId")?.Value);
            var operation = await _activityService.SetLog_Login(new UserIdDTO
            {
                UserId = userId,
            }, cancellationToken);
            return operation.Success ? Ok(operation) : StatusCode((int)operation.Status, operation);
        }
        [HttpPost("SetLog_Logout")]
        public async Task<ActionResult<OperationResult<object>>> SetLog_Logout(CancellationToken cancellationToken = default)
        {
            var userId = Convert.ToInt32(HttpContext.User.FindFirst("CoreUserId")?.Value);
            var operation = await _activityService.SetLog_Logout(new UserIdDTO
            {
                UserId = userId,
            }, cancellationToken);
            return operation.Success ? Ok(operation) : StatusCode((int)operation.Status, operation);
        }
        [HttpPost("SetLog_Action")]
        public async Task<ActionResult<OperationResult<object>>> SetLog_Action(SetLogDTO model, CancellationToken cancellationToken = default)
        {
            var userId = Convert.ToInt32(HttpContext.User.FindFirst("CoreUserId")?.Value);
            var operation = await _actionService.SetLog_Action(userId, model, cancellationToken);
            return operation.Success ? Ok(operation) : StatusCode((int)operation.Status, operation);
        }
    }
}
