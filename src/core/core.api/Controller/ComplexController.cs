using core.application.Contract.API.DTO.Complex;
using core.application.Contract.API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace core.web.api.Controllers
{
    [CustomAuthorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ComplexController : ControllerBase
    {
        private readonly IComplexService _complexService;

        public ComplexController(IComplexService complexService)
        {
            _complexService = complexService;
        }

        // GET: api/Complex
        [HttpGet("GetComplexes")]
        public async Task<ActionResult<IEnumerable<ComplexGetResponseDTO>>> GetComplexes()
        {
            var complexes = await _complexService.GetAllComplexesAsync();
            return Ok(complexes);
        }

        // GET: api/Complex/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ComplexGetResponseDTO>> GetComplex(int id)
        {
            var complex = await _complexService.GetComplexById(id);

            if (complex == null)
            {
                return NotFound();
            }

            return Ok(complex);
        }

        [HttpPost("CreateComplex")]
        public async Task<ActionResult<ComplexGetResponseDTO>> CreateComplex([FromBody] ComplextCreateRequestDTO complexCreateDTO)
        {
            int id = await _complexService.CreateComplex(complexCreateDTO);

            return Ok(id);
        }

        [HttpPut("UpdateComplex")]
        public async Task<IActionResult> UpdateComplex([FromBody] ComplexUpdateRequestDTO complexUpdateDTO)
        {
            if (complexUpdateDTO is null || complexUpdateDTO.Id <= 0)
            {
                return BadRequest();
            }

            bool updated = await _complexService.UpdateComplex(complexUpdateDTO);

            if (!updated)
            {
                return NotFound();
            }

            return Ok(updated);
        }
    }
}
