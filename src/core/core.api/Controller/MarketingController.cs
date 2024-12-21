using core.application.Contract.API.DTO.Marketing;
using core.application.Contract.API.Interfaces;
using core.domain.DomainModelDTOs.MIKAMarketingDTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace core.api.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class MarketingController : ControllerBase
    {
        #region Constructor of Marketing Service Controllers
        private readonly IMarketingService _marketingService;
        public MarketingController(IMarketingService marketingService)
        {
            _marketingService = marketingService;
        }
        #endregion
        #region Project/s
        [HttpPost("CreateProject")]
        public async Task<IActionResult> CreateProject([FromBody] Request_CreateProject project)
        {
            if (project == null)
                return BadRequest("Project data is null.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _marketingService.CreateProject(project);
            return result;
        }

        [HttpGet("GetProjects")]
        public async Task<IActionResult> GetProjects()
        {
            var result = await _marketingService.GetAllProjects();
            return result;
        }

        [HttpGet("GetProjectById/{id}")]
        public async Task<IActionResult> GetProjectById(int id)
        {
            var result = await _marketingService.GetProjectById(id);
            return result != null ? Ok(result) : NotFound();
        }

        [HttpPatch("UpdateProject/{id}")]
        public async Task<IActionResult> UpdateProject(int id, Request_UpdateProject updateProject)
        {
            if (updateProject == null)
                return BadRequest("Update project data is null.");

            var result = await _marketingService.UpdateProject(id, updateProject);
            return Ok(result);
        }

        [HttpPatch("UpdateProject/{projectId}/UpdateBlocks")]
        public async Task<IActionResult> UpdateBlocks(int projectId, Request_ProjectBlocks request)
        {
            if (request == null)
                return BadRequest("Update the field Block Details is not done yet!");

            var result = await _marketingService.UpdateBlocks(projectId, request);
            return Ok(result);
        }

        [HttpPatch("UpdateProject/{projectId}/UpdateUnits")]
        public async Task<IActionResult> UpdateUnits(int projectId, Request_UnitsProjects request)
        {
            if (request == null)
                return BadRequest("Update of Unit Details iks not OKAY !");

            var result = await _marketingService.UpdateUnits(projectId, request);
            return Ok(result);
        }

        [HttpPatch("DeleteProject/{id}")]
        public async Task<IActionResult> DeleteProject(int id, [FromBody] Request_DeleteProject deleteProject)
        {
            try
            {
                var result = await _marketingService.DeleteProject(id, deleteProject);
                if (result == null)
                {
                    return NotFound($"Project with ID {id} not found.");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpPatch("ActivateProject/{id}")]
        public async Task<IActionResult> ActivateProject(int id, [FromBody] Request_ActivateProject activateProject)
        {
            try
            {
                var result = await _marketingService.ActivateProject(id, activateProject);
                if (result != null)
                {
                    return BadRequest($"Project with ID {id} not found !");
                }
                return Ok(result);
            } catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("UploadFile")]
        public async Task<IActionResult> UploadProjectFiles([FromForm] ProjectFilesDTO projectFiles, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file provided!");
            }

            try
            {
                var result = await _marketingService.UploadProjectFiles(projectFiles, file);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet("GetProjectImages/{projectId}")]
        public async Task<IActionResult> GetProjectImages(int projectId)
        {
            if (projectId == 0)
                return BadRequest("No Project Id provided!");

            try
            {
                var result = await _marketingService.GetProjectImages(projectId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var ReadingStatusCode = this.HttpContext.Response.StatusCode;
                if(ReadingStatusCode == 500)
                    return StatusCode(500, $"Internal server error: {ex.Message}");

                if (ReadingStatusCode == 404)
                    return StatusCode(404, $"The Service is not found : {ex.Message}");

                if(ReadingStatusCode == 304)
                    return StatusCode(304, $"Service is Redirected : {ex.Message}");

                return BadRequest($"{ex.Message}");
            }
        }
        #endregion

        #region UserProfile
        [HttpPost("CreateProfile")]
        public async Task<IActionResult> CreateUserProfile(Request_CreateUserProfile createProfile)
        {
            var result = await _marketingService.CreateUserProfile(createProfile);
            return Ok(result);
        }

        [HttpPut("UpdateProfile/{userId}")]
        public async Task<IActionResult> UpdateProfile(int userId, Request_UpdateUserProfile updateUserProfile)
        {
            if (updateUserProfile == null)
                return BadRequest("Update profile data is null.");

            var result = await _marketingService.UpdateUserProfile(userId, updateUserProfile);
            return Ok(result);
        }

        [HttpDelete("DeleteProfile/{userId}")]
        public async Task<IActionResult> DeleteProfile(int userId)
        {
            var result = await _marketingService.DeleteUserProfile(userId);
            return Ok(result);
        }

        [HttpGet("GetUserProfile/{userId}")]
        public async Task<IActionResult> GetUserProfile(int userId)
        {
            var result = await _marketingService.GetUserProfile(userId);
            return result != null ? Ok(result) : NotFound();
        }

        [HttpGet("GetAllUserProfiles")]
        public async Task<IActionResult> GetAllUserProfiles()
        {
            var result = await _marketingService.GetAllUserProfiles();
            return Ok(result);
        }
        #endregion

        #region Booking
        [HttpPost("CreateBooking")]
        public async Task<IActionResult> CreateBooking(Request_CreateBooking createBooking)
        {
            var result = await _marketingService.CreateBooking(createBooking);
            return Ok(result);
        }

        [HttpPut("UpdateBooking/{bookingId}")]
        public async Task<IActionResult> UpdateBooking(int bookingId, Request_UpdateBooking updateBooking)
        {
            if (updateBooking == null)
                return BadRequest("Update booking data is null.");

            var result = await _marketingService.UpdateBooking(bookingId, updateBooking);
            return Ok(result);
        }

        [HttpDelete("DeleteBooking/{bookingId}")]
        public async Task<IActionResult> DeleteBooking(int bookingId)
        {
            var result = await _marketingService.DeleteBooking(bookingId);
            return Ok(result);
        }

        [HttpGet("GetBookingById/{bookingId}")]
        public async Task<IActionResult> GetBookingById(int bookingId)
        {
            var result = await _marketingService.GetBookingById(bookingId);
            return result != null ? Ok(result) : NotFound();
        }

        [HttpGet("GetAllBookings")]
        public async Task<IActionResult> GetAllBookings()
        {
            var result = await _marketingService.GetAllBookings();
            return Ok(result);
        }
        #endregion
    }
}
