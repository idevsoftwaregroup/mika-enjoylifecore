using IdentityProvider.Application.Contracts.Authentication.BackDoor;
using IdentityProvider.Application.Contracts.Authentication.Login;
using IdentityProvider.Application.Contracts.Authentication.OTPVerification;
using IdentityProvider.Application.Contracts.Authentication.Register;
using IdentityProvider.Application.Contracts.UserUpdate;
using IdentityProvider.Application.Framework;
using IdentityProvider.Application.Helper;
using IdentityProvider.Application.Interfaces;
using IdentityProvider.Domain.DomainModelDTOs;
using IdentityProvider.Domain.DomainModelDTOs.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Net;
using System.Security.Claims;

namespace IdentityProvider.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly ILogger<AuthenticationController> _logger;
        public AuthenticationController(IAuthenticationService authenticationService, ILogger<AuthenticationController> logger)
        {
            _authenticationService = authenticationService;
            _logger = logger;
        }

        //[Authorize(Roles = "Admin")]
        [HttpPost("Register")]
        public async Task<ActionResult<RegisterResponseDTO>> RegisterUser(RegisterRequestDTO requestDTO, CancellationToken cancellationToken)
        {
            return Ok(await _authenticationService.RegisterUser(requestDTO, cancellationToken));
        }

        [HttpGet]
        public async Task<IActionResult> SeedAdmin()
        {
            await _authenticationService.SeedAdmin();
            return Ok();
        }

        [HttpPost("LoginByPassword")]
        public async Task<ActionResult<OperationResult<UserInfoResponseDTO>>> LoginByPassword(Request_LoginByPasswrodDomainDTO model, CancellationToken cancellationToken = default)
        {
            var operation = await _authenticationService.LoginByPassword(model, cancellationToken);
            return operation.Success ? Ok(operation) : StatusCode((int)operation.Status, operation);
        }

        [HttpPost("LoginByOTP")]
        public async Task<ActionResult<OperationResult<object>>> LoginByOTP(LoginRequestDTO requestDTO, CancellationToken cancellationToken)
        {
            var operation = await _authenticationService.LoginByOTP(requestDTO, cancellationToken);
            return operation.Success ? Ok(operation) : StatusCode((int)operation.Status, operation);
        }

        [HttpPost("LoginLobbyMan")]
        public async Task<ActionResult<string>> LoginLobbyMan(Request_CoreId requestDTO, CancellationToken cancellationToken = default)
        {
            var operation = await _authenticationService.LoginLobbyMan(requestDTO.CoreId, cancellationToken);
            return operation.Success ? Ok(operation.Object.Token) : StatusCode((int)operation.Status, operation.Message);
        }

        [HttpPost("VerifyOTP")]
        public async Task<ActionResult<OperationResult<UserInfoResponseDTO>>> VerifyOTP(OTPVerificationRequestDTO requestDTO, CancellationToken cancellationToken)
        {
            var operation = await _authenticationService.VerifyOTP(requestDTO, cancellationToken);
            return operation.Success ? Ok(operation) : StatusCode((int)operation.Status, operation);
        }

        [Authorize]
        [HttpPost("CreateUser")]
        public async Task<ActionResult<OperationResult<object>>> CreateUser(Request_CreateUserDomainDTO requestDTO, CancellationToken cancellationToken = default)
        {
            var roleClaims = HttpContext.User.FindAll(ClaimTypes.Role);
            if (roleClaims == null || !roleClaims.Any() || !roleClaims.Select(x => Convert.ToString(x.Value)).ToList().Any(x => x.ToUpper() == "ADMIN"))
            {
                return StatusCode((int)HttpStatusCode.Forbidden, new OperationResult<object>("CreateUser").Failed("دسترسی ساخت کاربر جدید برای کاربری شما وجود ندارد", HttpStatusCode.Forbidden));
            }
            var operation = await _authenticationService.CreateUser(requestDTO, cancellationToken);
            return !operation.Success ? StatusCode((int)operation.Status, operation) : Ok(operation);
        }

        [Authorize]
        [HttpPost("CreateUsers")]
        public async Task<ActionResult<OperationResult<object>>> CreateUsers(List<Request_CreateUserDomainDTO> requestDTO, CancellationToken cancellationToken = default)
        {
            var roleClaims = HttpContext.User.FindAll(ClaimTypes.Role);
            if (roleClaims == null || !roleClaims.Any() || !roleClaims.Select(x => Convert.ToString(x.Value)).ToList().Any(x => x.ToUpper() == "ADMIN"))
            {
                return StatusCode((int)HttpStatusCode.Forbidden, new OperationResult<object>("CreateUsers").Failed("دسترسی ساخت گروهی کاربران برای کاربری شما وجود ندارد", HttpStatusCode.Forbidden));
            }
            var operation = await _authenticationService.CreateUsers(requestDTO, cancellationToken);
            return !operation.Success ? StatusCode((int)operation.Status, operation) : Ok(operation);
        }
        [Authorize]
        [HttpPut("UpdateUser")]
        public async Task<ActionResult<OperationResult<object>>> UpdateUser(Request_UpdateUserDomainDTO requestDTO, CancellationToken cancellationToken = default)
        {
            var roleClaims = HttpContext.User.FindAll(ClaimTypes.Role);
            if (roleClaims == null || !roleClaims.Any() || !roleClaims.Select(x => Convert.ToString(x.Value)).ToList().Any(x => x.ToUpper() == "ADMIN"))
            {
                return StatusCode((int)HttpStatusCode.Forbidden, new OperationResult<object>("UpdateUser").Failed("دسترسی بروزرسانی اطلاعات کاربر برای کاربری شما وجود ندارد", HttpStatusCode.Forbidden));
            }
            var operation = await _authenticationService.UpdateUser(requestDTO, cancellationToken);
            return !operation.Success ? StatusCode((int)operation.Status, operation) : Ok(operation);
        }
        [Authorize]
        [HttpDelete("DeleteUser/{CoreID}")]
        public async Task<ActionResult<OperationResult<object>>> DeleteUser(int CoreID, CancellationToken cancellationToken = default)
        {
            var roleClaims = HttpContext.User.FindAll(ClaimTypes.Role);
            if (roleClaims == null || !roleClaims.Any() || !roleClaims.Select(x => Convert.ToString(x.Value)).ToList().Any(x => x.ToUpper() == "ADMIN"))
            {
                return StatusCode((int)HttpStatusCode.Forbidden, new OperationResult<object>("UpdateUser").Failed("دسترسی بروزرسانی اطلاعات کاربر برای کاربری شما وجود ندارد", HttpStatusCode.Forbidden));
            }
            var operation = await _authenticationService.DeleteUser(new Request_DeleteUserDomainDTO
            {
                CoreId = CoreID,
            }, cancellationToken);
            return !operation.Success ? StatusCode((int)operation.Status, operation) : Ok(operation);
        }
        [CustomAuthorize]
        [HttpPost("CreateOrUpdateIdentityUser")]
        public async Task<ActionResult<OperationResult<object>>> CreateOrUpdateIdentityUser(Request_CreateOrUpdateIdentityUserDomainDTO model, CancellationToken cancellationToken = default)
        {
            var roleClaims = HttpContext.User.FindAll(ClaimTypes.Role);
            if (roleClaims == null || !roleClaims.Any() || !roleClaims.Select(x => Convert.ToString(x.Value)).ToList().Any(x => x.ToUpper() == "ADMIN"))
            {
                return StatusCode((int)HttpStatusCode.Forbidden, new OperationResult<object>("CreateOrUpdateIdentityUser").Failed("دسترسی برای کاربری شما وجود ندارد", HttpStatusCode.Forbidden));
            }
            var operation = await _authenticationService.CreateOrUpdateIdentityUser(model, cancellationToken);
            return !operation.Success ? StatusCode((int)operation.Status, operation) : Ok(operation);
        }
        [CustomAuthorize]
        [HttpPost("DeleteIdentityUser")]
        public async Task<ActionResult<OperationResult<object>>> DeleteIdentityUser(Request_CoreId model, CancellationToken cancellationToken = default)
        {
            var roleClaims = HttpContext.User.FindAll(ClaimTypes.Role);
            if (roleClaims == null || !roleClaims.Any() || !roleClaims.Select(x => Convert.ToString(x.Value)).ToList().Any(x => x.ToUpper() == "ADMIN"))
            {
                return StatusCode((int)HttpStatusCode.Forbidden, new OperationResult<object>("DeleteIdentityUser").Failed("دسترسی برای کاربری شما وجود ندارد", HttpStatusCode.Forbidden));
            }
            var operation = await _authenticationService.DeleteIdentityUser(model, cancellationToken);
            return !operation.Success ? StatusCode((int)operation.Status, operation) : Ok(operation);
        }


    }
}
