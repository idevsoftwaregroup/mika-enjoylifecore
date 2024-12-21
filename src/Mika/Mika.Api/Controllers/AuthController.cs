using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mika.Api.Helpers.Services;
using Mika.Application.Services.Interfaces;
using Mika.Domain.Contracts.DTOs.Users;
using Mika.Framework.Models;

namespace Mika.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserApplication _userApplication;
        private readonly TokenHelper _tokenHelper;
        public AuthController(IUserApplication userApplication, TokenHelper tokenHelper)
        {
            this._userApplication = userApplication;
            this._tokenHelper = tokenHelper;
        }
        [HttpPost("Login")]
        public async Task<ActionResult<OperationResult<Response_AuthenticationDTO>>> Login(Request_LoginDTO model, CancellationToken cancellationToken = default)
        {
            try
            {
                var operation = await _userApplication.Login(model, cancellationToken);
                if (!operation.Success)
                {
                    return StatusCode((int)operation.Status, operation);
                }
                var responseData = operation.Object;
                var tokenData = this._tokenHelper.GenerateToken(responseData.UserId, responseData.UserName, responseData.RoleName, responseData.Modules);
                responseData.Token = tokenData.Token;
                responseData.TokenExpirationDate = tokenData.ExpirationDate;
                return Ok(new OperationResult<Response_AuthenticationDTO>("Login").Succeed(operation.Message, responseData));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new OperationResult<Response_AuthenticationDTO>("Login").Failed("ورود با مشکل مواجه شده است", ex.Message, System.Net.HttpStatusCode.InternalServerError));
            }

        }

        [Authorize]
        [HttpPut("UpdateProfile")]
        public async Task<ActionResult<OperationResult<Response_AuthenticationDTO>>> UpdateProfile(Request_UpdateSelfUserDTO model, CancellationToken cancellationToken = default)
        {
            var UserId = Convert.ToInt64(HttpContext.User.Claims.First(x => x.Type == "UserId")?.Value);
            try
            {
                var operation = await _userApplication.UpdateProfile(UserId, model, cancellationToken);
                if (!operation.Success)
                {
                    return StatusCode((int)operation.Status, operation);
                }
                var responseData = operation.Object;
                var tokenData = this._tokenHelper.GenerateToken(responseData.UserId, responseData.UserName, responseData.RoleName, responseData.Modules);
                responseData.Token = tokenData.Token;
                responseData.TokenExpirationDate = tokenData.ExpirationDate;
                return Ok(new OperationResult<Response_AuthenticationDTO>("UpdateProfile").Succeed(operation.Message, responseData));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new OperationResult<Response_AuthenticationDTO>("UpdateProfile").Failed("بروزرسانی با مشکل مواجه شده است", ex.Message, System.Net.HttpStatusCode.InternalServerError));
            }
        }
    }
}
