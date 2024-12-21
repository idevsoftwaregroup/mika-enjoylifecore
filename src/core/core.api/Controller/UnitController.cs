using core.application.Contract.API.DTO.Party.User;
using core.application.Contract.API.DTO.Structor.Unit;
using core.application.Contract.API.Interfaces;
using core.application.Framework;
using core.application.Services;
using core.domain.DomainModelDTOs.TicketingDTOs;
using core.domain.DomainModelDTOs.Units_Residents_OwnersDTOs;
using core.domain.DomainModelDTOs.Units_Residents_OwnersDTOs.FilterDTOs;
using core.domain.entity.structureModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;

namespace core.api.Controller
{
    [CustomAuthorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UnitController : ControllerBase
    {
        private readonly IUnitService _unitService;

        public UnitController(IUnitService unitService)
        {
            _unitService = unitService;
        }

        [HttpPost("GetAllUnitsInComplex")]
        public async Task<ActionResult<IEnumerable<UnitResponseDTO>>> GetAllUnitsInComplex([FromBody] UnitRequestDTO filter)
        {
            return Ok(await _unitService.getAllunit(filter));
        }

        [HttpGet]
        [Route("GetUserUnits")]
        public async Task<ActionResult<IEnumerable<UnitUserResponseDTO>>> GetUserUnits(bool IsHead = true)
        {
            try
            {
                var userId = Convert.ToInt32(HttpContext.User.FindFirst("CoreUserId")?.Value);
                var units = await _unitService.GetUserUnits(userId, IsHead);

                if (units == null)
                {
                    return NotFound();
                }

                return Ok(units);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }

        }
        [HttpGet]
        [Route("GetMyResidentalUnits")]
        public async Task<ActionResult<IEnumerable<UnitUserResponseDTO>>> GetMyResidentalUnits()
        {
            try
            {
                var userId = Convert.ToInt32(HttpContext.User.FindFirst("CoreUserId")?.Value);
                var units = await _unitService.GetMyResidentalUnits(userId);

                if (units == null)
                {
                    return NotFound();
                }

                return Ok(units);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }

        }
        /// <summary>
        /// //////////////////////////////
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAllUsersInUnit")]

        public async Task<ActionResult<IEnumerable<UnitUserResponseDTO>>> GetAllUsersInUnit()
        {
            try
            {
                var userId = Convert.ToInt32(HttpContext.User.FindFirst("CoreUserId")?.Value);
                var units = await _unitService.GetAllUsersInUnit(userId);

                if (units == null || !units.Any())
                {
                    return NoContent();
                }

                return Ok(units);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpGet("{unitId}", Name = "GetUnitById")]
        public async Task<ActionResult<FrontGetUnitDTO>> GetUnitById(int unitId)
        {


            return Ok(await _unitService.GetUnitByIdFront(unitId));
        }

        [HttpGet("{unitId}/Admin")]
        public async Task<ActionResult<UnitResponseDTO>> GetUnitByIdAdmin(int unitId)
        {
            return Ok(await _unitService.GetUnitById(unitId));
        }

        [HttpGet("GetUnitOwnersResidents")]
        public async Task<ActionResult<OperationResult<Response_GetUnitOwnersResidentsDomainDTO>>> GetUnitOwnersResidents([FromQuery] Filter_GetUnitOwnersResidentsDomainDTO? filter, CancellationToken cancellationToken = default)
        {
            var roleClaims = HttpContext.User.FindAll(ClaimTypes.Role);
            if (roleClaims == null || !roleClaims.Any() || !roleClaims.Select(x => Convert.ToString(x.Value)).ToList().Any(x => x.ToUpper() == "ADMIN"))
            {
                return StatusCode((int)HttpStatusCode.Forbidden, new OperationResult<Response_GetUnitOwnersResidentsDomainDTO>("GetUnitOwnersResidents").Failed("دسترسی فراخوانی اطلاعات برای کاربری شما وجود ندارد", HttpStatusCode.Forbidden));
            }
            var operation = await _unitService.GetUnitOwnersResidents(filter, cancellationToken);
            return !operation.Success ? StatusCode((int)operation.Status, operation) : Ok(operation);
        }

        [HttpPost("CreateUnit")]
        public async Task<IActionResult> CreateUnit([FromBody] UnitCreateRequestDTO unitCreateDTO)
        {
            var unitId = await _unitService.CreateUnit(unitCreateDTO);
            if (unitId > 0)
            {
                return CreatedAtRoute("GetUnitById", new { unitId = unitId }, unitId);
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPut("UpdateUnit")]
        public async Task<IActionResult> UpdateUnit([FromBody] UnitUpdateRequestDTO unitUpdateDTO)
        {
            if (unitUpdateDTO is null || unitUpdateDTO.Id <= 0)
            {
                return BadRequest();
            }
            if (await _unitService.UpdateUnit(unitUpdateDTO))
            {
                return Ok(unitUpdateDTO.Id); // should be get dto probably
            }
            else
            {
                return BadRequest();
            }
        }
        [HttpDelete("Parking")]
        public async Task<IActionResult> DeleteParking(int id)
        {
            await _unitService.DeleteParking(id);
            return NoContent();
        }
        [HttpPost("Parking")]
        public async Task<ActionResult<Parking>> AddParking(AddParkingDTO parkingDTO)
        {
            return Ok(await _unitService.AddParking(parkingDTO));
        }
        [HttpDelete("StorageLot")]
        public async Task<IActionResult> DeleteStorageLot(int id)
        {
            await _unitService.DeleteStorageLot(id);
            return NoContent();
        }
        [HttpPost("StorageLot")]
        public async Task<ActionResult<StorageLot>> AddStorageLot(AddStorageLotDTO storageLotDTO)
        {
            return Ok(await _unitService.AddStorageLot(storageLotDTO));
        }
    }
}
