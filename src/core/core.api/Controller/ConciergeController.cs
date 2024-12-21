using core.application.Contract.API.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace core.api.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConciergeController : ControllerBase
    {
        private readonly IConciergeService _conciergeService;
        public ConciergeController(IConciergeService concierge)
        {
            _conciergeService = concierge;
        }
        [HttpGet("GetConcierge")]
        public async Task<IActionResult> GetSingleConcierge(CancellationToken cancellationToken)
        {
            try
            {
                var result = await _conciergeService.GetConcierge(cancellationToken);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

    }
}
