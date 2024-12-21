using core.application.Contract.API.DTO.LobbyAttendant;
using core.application.Contract.API.DTO.LobbyAttendant.Filter;
using core.application.Contract.API.Interfaces;
using core.application.Contract.infrastructure.Services;
using core.application.Framework;
using IdentityProvider.Application.Interfaces.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;

namespace core.api.Controller
{
    [CustomAuthorize]
    [Route("api/[controller]")]
    [ApiController]
    public class LobbyAttendantController : ControllerBase
    {
        private readonly ILobbyAttendantService _lobbyAttendantService;
        private readonly IIdentityService _identityService;
        public LobbyAttendantController(ILobbyAttendantService lobbyAttendantService, IIdentityService identityService)
        {
            this._lobbyAttendantService = lobbyAttendantService;
            this._identityService = identityService;
        }

        [AllowAnonymous]
        [HttpPost("LoginLobbyAttendantFirstStep")]
        public async Task<ActionResult<OperationResult<object>>> LoginLobbyAttendantFirstStep(Request_LoginLobbyAttendantDTO model, CancellationToken cancellationToken = default)
        {
            var operation = await _lobbyAttendantService.LoginLobbyAttendant(model, cancellationToken);
            return operation.Success ? Ok(operation) : StatusCode((int)operation.Status, operation);
        }

        [AllowAnonymous]
        [HttpPost("LoginLobbyAttendantSecondStep")]
        public async Task<ActionResult<OperationResult<Response_LoginLobbyAttendantDTO>>> LoginLobbyAttendantSecondStep(Request_LobbyAttendandGetTokenDTO model, CancellationToken cancellationToken = default)
        {
            var loginOperation = await _lobbyAttendantService.SecondLoginLobbyAttendant(model, cancellationToken);
            return loginOperation.Success ? Ok(loginOperation) : StatusCode((int)loginOperation.Status, loginOperation);
        }

        [HttpPost("LoginLobbyAttendantSecondStep_Authorized")]
        public async Task<ActionResult<OperationResult<Response_LoginLobbyAttendantDTO>>> LoginLobbyAttendantSecondStep_Authorized(Request_LobbyAttendantUserIdDTO model, CancellationToken cancellationToken = default)
        {
            var getDataOperation = await _lobbyAttendantService.SecondLoginLobbyAttendant_Authorized(model, cancellationToken);
            return getDataOperation.Success ? Ok(getDataOperation) : StatusCode((int)getDataOperation.Status, getDataOperation);
        }

        [AllowAnonymous]
        [HttpPost("GetUnitsForLobbyAttendant")]
        public async Task<ActionResult<OperationResult<Response_LobbyAttendantUnitsDTO>>> GetUnitsForLobbyAttendant(Request_LoginLobbyAttendantDTO model, CancellationToken cancellationToken = default)
        {
            var operation = await _lobbyAttendantService.GetUnitsForLobbyAttendant(model, cancellationToken);
            return operation.Success ? Ok(operation) : StatusCode((int)operation.Status, operation);
        }

        [HttpGet("GetUnitsForLobbyAttendant_Authorized")]
        public async Task<ActionResult<OperationResult<Response_LobbyAttendantUnitsDTO>>> GetUnitsForLobbyAttendant_Authorized(CancellationToken cancellationToken = default)
        {
            var operation = await _lobbyAttendantService.GetUnitsForLobbyAttendant_Authorized(cancellationToken);
            return operation.Success ? Ok(operation) : StatusCode((int)operation.Status, operation);
        }

        [AllowAnonymous]
        [HttpPost("GetUsersInUnitForLobbyAttendant/{UnitId}")]
        public async Task<ActionResult<OperationResult<Response_LobbyAttendantUsersInUnitDTO>>> GetUsersInUnitForLobbyAttendant(int UnitId, Request_LoginLobbyAttendantDTO model, CancellationToken cancellationToken = default)
        {
            var operation = await _lobbyAttendantService.GetUsersInUnitForLobbyAttendant(UnitId, model, cancellationToken);
            return operation.Success ? Ok(operation) : StatusCode((int)operation.Status, operation);
        }

        [HttpGet("GetUsersInUnitForLobbyAttendant_Authorized/{UnitId}")]
        public async Task<ActionResult<OperationResult<Response_LobbyAttendantUsersInUnitDTO>>> GetUsersInUnitForLobbyAttendant_Authorized(int UnitId, CancellationToken cancellationToken = default)
        {
            var operation = await _lobbyAttendantService.GetUsersInUnitForLobbyAttendant_Authorized(UnitId, cancellationToken);
            return operation.Success ? Ok(operation) : StatusCode((int)operation.Status, operation);
        }

        [HttpGet("GetLobbyAttendants")]
        public async Task<ActionResult<OperationResult<Reponse_GetLobbyAttendantDTO>>> GetLobbyAttendants([FromQuery] Filter_GetLobbyAttendantDTO filter, CancellationToken cancellationToken = default)
        {
            var roleClaims = HttpContext.User.FindAll(ClaimTypes.Role);
            if (roleClaims == null || !roleClaims.Any() || !roleClaims.Select(x => Convert.ToString(x.Value)).ToList().Any(x => x.ToUpper() == "ADMIN"))
            {
                return StatusCode((int)HttpStatusCode.Forbidden, new OperationResult<object>("GetLobbyAttendants").Failed("دسترسی عملیات برای کاربری شما وجود ندارد", HttpStatusCode.Forbidden));
            }
            var operation = await _lobbyAttendantService.GetLobbyAttendants(filter, cancellationToken);
            return operation.Success ? Ok(operation) : StatusCode((int)operation.Status, operation);
        }

        [HttpPost("CreateLobbyAttendant")]
        public async Task<ActionResult<OperationResult<object>>> CreateLobbyAttendant(Request_CreateLobbyAttendantDTO model, CancellationToken cancellationToken = default)
        {
            var roleClaims = HttpContext.User.FindAll(ClaimTypes.Role);
            if (roleClaims == null || !roleClaims.Any() || !roleClaims.Select(x => Convert.ToString(x.Value)).ToList().Any(x => x.ToUpper() == "ADMIN"))
            {
                return StatusCode((int)HttpStatusCode.Forbidden, new OperationResult<object>("CreateLobbyAttendant").Failed("دسترسی عملیات برای کاربری شما وجود ندارد", HttpStatusCode.Forbidden));
            }
            var operation = await _lobbyAttendantService.CreateLobbyAttendant(Convert.ToInt32(HttpContext.User.FindFirst("CoreUserId")?.Value), model, cancellationToken);
            return operation.Success ? Ok(operation) : StatusCode((int)operation.Status, operation);
        }

        [HttpPut("UpdateLobbyAttendant")]
        public async Task<ActionResult<OperationResult<object>>> UpdateLobbyAttendant(Request_UpdateLobbyAttendantDTO model, CancellationToken cancellationToken = default)
        {
            var roleClaims = HttpContext.User.FindAll(ClaimTypes.Role);
            if (roleClaims == null || !roleClaims.Any() || !roleClaims.Select(x => Convert.ToString(x.Value)).ToList().Any(x => x.ToUpper() == "ADMIN"))
            {
                return StatusCode((int)HttpStatusCode.Forbidden, new OperationResult<object>("UpdateLobbyAttendant").Failed("دسترسی عملیات برای کاربری شما وجود ندارد", HttpStatusCode.Forbidden));
            }
            var operation = await _lobbyAttendantService.UpdateLobbyAttendant(Convert.ToInt32(HttpContext.User.FindFirst("CoreUserId")?.Value), model, cancellationToken);
            return operation.Success ? Ok(operation) : StatusCode((int)operation.Status, operation);
        }

        [HttpDelete("DeleteLobbyAttendant")]
        public async Task<ActionResult<OperationResult<object>>> DeleteLobbyAttendant(Request_LobbyAttendantIdDTO model, CancellationToken cancellationToken = default)
        {
            var roleClaims = HttpContext.User.FindAll(ClaimTypes.Role);
            if (roleClaims == null || !roleClaims.Any() || !roleClaims.Select(x => Convert.ToString(x.Value)).ToList().Any(x => x.ToUpper() == "ADMIN"))
            {
                return StatusCode((int)HttpStatusCode.Forbidden, new OperationResult<object>("DeleteLobbyAttendant").Failed("دسترسی عملیات برای کاربری شما وجود ندارد", HttpStatusCode.Forbidden));
            }
            var operation = await _lobbyAttendantService.DeleteLobbyAttendant(model, cancellationToken);
            return operation.Success ? Ok(operation) : StatusCode((int)operation.Status, operation);
        }



    }
}
