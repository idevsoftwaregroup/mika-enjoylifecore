using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mika.Application.Services.Interfaces;
using Mika.Domain.Contracts.DTOs.Account;
using Mika.Domain.Contracts.DTOs.Account.Filter;
using Mika.Domain.Contracts.DTOs.Company;
using Mika.Domain.Contracts.DTOs.Company.Filter;
using Mika.Domain.Contracts.DTOs.Modules;
using Mika.Domain.Contracts.DTOs.Role;
using Mika.Framework.Models;
using Mika.Framework.Models.Pagination;
using System.Threading;
namespace Mika.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SettingController : ControllerBase
    {
        private readonly ISettingApplication _settingApplication;
        public SettingController(ISettingApplication settingApplication)
        {
            this._settingApplication = settingApplication;
        }
        [HttpGet("GetRoles")]
        public async Task<ActionResult<OperationResult<Response_GetRoleDTO>>> GetRoles(CancellationToken cancellationToken = default)
        {
            var UserId = Convert.ToInt64(HttpContext.User.Claims.First(x => x.Type == "UserId")?.Value);
            var operation = await _settingApplication.GetRoles(UserId, cancellationToken);
            return operation.Success ? Ok(operation) : StatusCode((int)operation.Status, operation);
        }

        [HttpGet("GetModules")]
        public async Task<ActionResult<OperationResult<Response_GetModuleDTO>>> GetModules(CancellationToken cancellationToken = default)
        {
            var UserId = Convert.ToInt64(HttpContext.User.Claims.First(x => x.Type == "UserId")?.Value);
            var operation = await _settingApplication.GetModules(UserId, cancellationToken);
            return operation.Success ? Ok(operation) : StatusCode((int)operation.Status, operation);
        }

        [HttpPut("UpdateModule")]
        public async Task<ActionResult<OperationResult<object>>> UpdateModule(Request_UpdateModuleDTO model, CancellationToken cancellationToken = default)
        {
            var UserId = Convert.ToInt64(HttpContext.User.Claims.First(x => x.Type == "UserId")?.Value);
            var operation = await _settingApplication.UpdateModule(UserId, model, cancellationToken);
            return operation.Success ? Ok(operation) : StatusCode((int)operation.Status, operation);
        }

        [HttpGet("GetCompanies")]
        public async Task<ActionResult<OperationResult<PagedList<Respone_GetCompanyDTO>>>> GetCompanies([FromQuery] Filter_GetCompanyDTO? filter, [FromQuery] PageModel? page, CancellationToken cancellationToken = default)
        {
            var UserId = Convert.ToInt64(HttpContext.User.Claims.First(x => x.Type == "UserId")?.Value);
            var operation = await _settingApplication.GetCompanies(UserId, filter, page, cancellationToken);
            return operation.Success ? Ok(operation) : StatusCode((int)operation.Status, operation);
        }

        [HttpGet("GetCompany/{CompanyId}")]
        public async Task<ActionResult<OperationResult<Respone_GetCompanyDTO>>> GetCompany(long CompanyId, CancellationToken cancellationToken = default)
        {
            var UserId = Convert.ToInt64(HttpContext.User.Claims.First(x => x.Type == "UserId")?.Value);
            var operation = await _settingApplication.GetCompany(UserId, CompanyId, cancellationToken);
            return operation.Success ? Ok(operation) : StatusCode((int)operation.Status, operation);
        }

        [HttpPost("CreateCompany")]
        public async Task<ActionResult<OperationResult<object>>> CreateCompany(Request_CreateCompanyDTO model, CancellationToken cancellationToken = default)
        {
            var UserId = Convert.ToInt64(HttpContext.User.Claims.First(x => x.Type == "UserId")?.Value);
            var operation = await _settingApplication.CreateCompany(UserId, model, cancellationToken);
            return operation.Success ? Ok(operation) : StatusCode((int)operation.Status, operation);
        }

        [HttpPut("UpdateCompany")]
        public async Task<ActionResult<OperationResult<object>>> UpdateCompany(Request_UpdateCompanyDTO model, CancellationToken cancellationToken = default)
        {
            var UserId = Convert.ToInt64(HttpContext.User.Claims.First(x => x.Type == "UserId")?.Value);
            var operation = await _settingApplication.UpdateCompany(UserId, model, cancellationToken);
            return operation.Success ? Ok(operation) : StatusCode((int)operation.Status, operation);
        }

        [HttpDelete("DeleteCompany")]
        public async Task<ActionResult<OperationResult<object>>> DeleteCompany(Request_DeleteCompanyDTO model, CancellationToken cancellationToken = default)
        {
            var UserId = Convert.ToInt64(HttpContext.User.Claims.First(x => x.Type == "UserId")?.Value);
            var operation = await _settingApplication.DeleteCompany(UserId, model, cancellationToken);
            return operation.Success ? Ok(operation) : StatusCode((int)operation.Status, operation);
        }

        [HttpGet("GetAccounts")]
        public async Task<ActionResult<OperationResult<PagedList<Response_GetAccountDTO>>>> GetAccounts([FromQuery] Filter_GetAccountDTO? filter, [FromQuery] PageModel? page, CancellationToken cancellationToken = default)
        {
            var UserId = Convert.ToInt64(HttpContext.User.Claims.First(x => x.Type == "UserId")?.Value);
            var operation = await _settingApplication.GetAccounts(UserId, filter, page, cancellationToken);
            return operation.Success ? Ok(operation) : StatusCode((int)operation.Status, operation);
        }

        [HttpGet("GetAccount/{AccountId}")]
        public async Task<ActionResult<OperationResult<Response_GetAccountDTO>>> GetAccount(long AccountId, CancellationToken cancellationToken = default)
        {
            var UserId = Convert.ToInt64(HttpContext.User.Claims.First(x => x.Type == "UserId")?.Value);
            var operation = await _settingApplication.GetAccount(UserId, AccountId, cancellationToken);
            return operation.Success ? Ok(operation) : StatusCode((int)operation.Status, operation);
        }

        [HttpPost("CreateAccount")]
        public async Task<ActionResult<OperationResult<object>>> CreateAccount(Request_CreateAccountDTO model, CancellationToken cancellationToken = default)
        {
            var UserId = Convert.ToInt64(HttpContext.User.Claims.First(x => x.Type == "UserId")?.Value);
            var operation = await _settingApplication.CreateAccount(UserId, model, cancellationToken);
            return operation.Success ? Ok(operation) : StatusCode((int)operation.Status, operation);
        }

        [HttpPut("UpdateAccount")]
        public async Task<ActionResult<OperationResult<object>>> UpdateAccount(Request_UpdateAccountDTO model, CancellationToken cancellationToken = default)
        {
            var UserId = Convert.ToInt64(HttpContext.User.Claims.First(x => x.Type == "UserId")?.Value);
            var operation = await _settingApplication.UpdateAccount(UserId, model, cancellationToken);
            return operation.Success ? Ok(operation) : StatusCode((int)operation.Status, operation);
        }

        [HttpDelete("DeleteAccount")]
        public async Task<ActionResult<OperationResult<object>>> DeleteAccount(Request_AccountIdDTO model, CancellationToken cancellationToken = default)
        {
            var UserId = Convert.ToInt64(HttpContext.User.Claims.First(x => x.Type == "UserId")?.Value);
            var operation = await _settingApplication.DeleteAccount(UserId, model, cancellationToken);
            return operation.Success ? Ok(operation) : StatusCode((int)operation.Status, operation);
        }
    }
}
