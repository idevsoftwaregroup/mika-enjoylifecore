using core.application.Contract.API.DTO.Party.Resident;
using core.application.Contract.API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace core.web.api.Controllers
{
    [CustomAuthorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ResidentController : ControllerBase
    {
        private readonly IResidentService _residentService;

        public ResidentController(IResidentService residentService)
        {
            _residentService = residentService;
        }


        [HttpPost("GetResidents")]
        public async Task<ActionResult<IEnumerable<ResidentGetResponse>>> GetResidents([FromBody] ResidentGetRequestFilter filter)
        {
            var residents = await _residentService.GetAllResidentsAsync(filter);
            return Ok(residents);
        }


        [HttpPost("CreateResident")]
        public async Task<ActionResult<ResidentGetResponse>> CreateResident([FromBody] ResidentCreateRequest residentCreateRequest)
        {
            int id = await _residentService.CreateResident(residentCreateRequest);

            return Ok(id);
        }


        [HttpPut("UpdateResident")]
        public async Task<IActionResult> UpdateResident([FromBody] ResidentUpdateRequest residentUpdateRequest)
        {
            if (residentUpdateRequest is null || residentUpdateRequest.Id <= 0)
            {
                return BadRequest();
            }

            bool updated = await _residentService.UpdateResident(residentUpdateRequest);

            if (!updated)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
