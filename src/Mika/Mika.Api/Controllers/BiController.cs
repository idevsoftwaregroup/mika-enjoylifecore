using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mika.Application.Services.Implementations;
using Mika.Application.Services.Interfaces;
using Mika.Domain.Contracts.DTOs.BiDataEntry;
using Mika.Domain.Contracts.DTOs.BiDataEntry.Filter;
using Mika.Framework.Models;
using Mika.Framework.Models.Pagination;
namespace Mika.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BiController : ControllerBase
    {
        private readonly IBiApplication _biApplication;
        public BiController(IBiApplication biApplication)
        {
            this._biApplication = biApplication;
        }
        [HttpGet("GetBiDataEntries")]
        public async Task<ActionResult<OperationResult<PagedList<Response_GetBiDataEntryDTO>>>> GetBiDataEntries([FromQuery] Filter_GetBiDataEntryDTO? filter, [FromQuery] PageModel? page, CancellationToken cancellationToken = default)
        {
            var UserId = Convert.ToInt64(HttpContext.User.Claims.First(x => x.Type == "UserId")?.Value);
            var operation = await _biApplication.GetBiDataEntries(UserId, filter, page, cancellationToken);
            return operation.Success ? Ok(operation) : StatusCode((int)operation.Status, operation);
        }

        [HttpPost("CreateBiDataEntry")]
        public async Task<ActionResult<OperationResult<object>>> CreateBiDataEntry(Request_CreateBiDataEntryDTO model, CancellationToken cancellationToken = default)
        {
            var CreatorId = Convert.ToInt64(HttpContext.User.Claims.First(x => x.Type == "UserId")?.Value);
            var operation = await _biApplication.CreateBiDataEntry(CreatorId, model, cancellationToken);
            return operation.Success ? Ok(operation) : StatusCode((int)operation.Status, operation);
        }
    }
}
