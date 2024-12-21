using core.application.Contract.API.DTO.Party.Manager;
using core.application.Contract.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace core.web.api.Controllers;
[CustomAuthorize]
[Route("api/[controller]")]
[ApiController]
public class ManagerController : ControllerBase
{
    private readonly IManagerService _managerService;

    public ManagerController(IManagerService managerService)
    {
        _managerService = managerService;
    }

    // GET: api/Manager/Complex/5
    [HttpGet("Complex/{complexId}")]
    public ActionResult<IEnumerable<ManagerGetResponse>> GetManagersByComplex(int complexId)
    {
        var managers = _managerService.GetAllManagersAsync(complexId);
        return Ok(managers);
    }

    // GET: api/Manager/5
    [HttpGet("{id}")]
    public ActionResult<ManagerGetResponse> GetManager(int id)
    {
        var manager = _managerService.GetManagerById(id);

        if (manager == null)
        {
            return NotFound();
        }

        return Ok(manager);
    }


    [HttpPost("CreateManager")]
    public async Task<ActionResult<ManagerGetResponse>> CreateManager(ManagerCreateRequest managerCreateRequest)
    {
        int id = await _managerService.CreateManager(managerCreateRequest);

        var manager = _managerService.GetManagerById(id);

        return CreatedAtAction("GetManager", new { id = manager.Id }, manager);
    }


    [HttpPut("UpdateManager")]
    public async Task<IActionResult> UpdateManager(ManagerUpdateRequest managerUpdateRequest)
    {
        if (managerUpdateRequest is null || managerUpdateRequest.Id <= 0)
        {
            return BadRequest();
        }
        bool updated = await _managerService.UpdateManager(managerUpdateRequest);
        if (!updated)
        {
            return NotFound();
        }
        return NoContent();
    }
}
