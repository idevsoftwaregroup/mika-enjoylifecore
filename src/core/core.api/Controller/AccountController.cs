using core.application.Contract.API.DTO.Account;
using core.application.Contract.API.DTO.Complex;
using core.application.Contract.API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;

namespace core.web.api.Controllers
{
    [CustomAuthorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }
        [HttpGet]
        public async Task<ActionResult<AccountGetResponseDTO>> GetAccount(int ComplexId, int? AccountType)
        {
            var _account = await _accountService.GetAccount(ComplexId, AccountType);

            if (_account == null)
            {
                return NotFound();
            }

            return Ok(_account);
        }

        [HttpGet("GetUserMenu")]
        public async Task<ActionResult<object>> GetUserMenu(CancellationToken cancellationToken = default)
        {
            var roleClaims = HttpContext.User.FindAll(ClaimTypes.Role);
            if (roleClaims == null || !roleClaims.Any())
            {
                return NoContent();
            }
            var result = await _accountService.GetUserMenu(roleClaims.Select(x => Convert.ToString(x.Value)).ToList(), cancellationToken);
            return result == null || !result.Any() ? NoContent() : Ok(result);
        }

    }
}
