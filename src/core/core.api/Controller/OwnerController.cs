using core.application.Contract.API.DTO.Party.Owner;
using core.application.Contract.API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace core.web.api.Controllers
{
    [CustomAuthorize]
    [Route("api/[controller]")]
    [ApiController]
    public class OwnerController : ControllerBase
    {
        private readonly IOwnerService _ownerService;

        public OwnerController(IOwnerService ownerService)
        {
            _ownerService = ownerService;
        }

        [HttpGet("GetOwners")]
        public async Task<ActionResult<IEnumerable<OwnerGetResponse>>> GetOwners([FromQuery] OwnerGetRequestFilter filter)
        {
            var owners = await _ownerService.GetAllOwnersAsync(filter);
            return Ok(owners);
        }

        [HttpPost("CreateOwner")]
        public async Task<ActionResult<OwnerGetResponse>> CreateOwner(OwnerCreateRequest ownerCreateDTO)
        {
            int id = await _ownerService.CreateOwner(ownerCreateDTO);

            return Ok(id);
        }

        [HttpPut("UpdateOwner")]
        public async Task<IActionResult> UpdateOwner(OwnerUpdateRequest ownerUpdateDTO)
        {
            if (ownerUpdateDTO is null || ownerUpdateDTO.Id <= 0)
            {
                return BadRequest();
            }

            bool updated = await _ownerService.UpdateOwner(ownerUpdateDTO);

            if (!updated)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
