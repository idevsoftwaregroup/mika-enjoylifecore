using core.application.Contract.API.DTO.Marketing;
using core.application.Contract.API.Interfaces;
using core.application.Contract.infrastructure;
using core.domain.DomainModelDTOs.MIKAMarketingDTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.application.Services
{
    public class MarketingService : IMarketingService
    {
        private readonly IMarketingRepository _marketingRespository;
        public MarketingService (IMarketingRepository marketingRepository)
        {
            _marketingRespository = marketingRepository;
        }
        #region Projects
        public async Task<IActionResult> CreateProject(Request_CreateProject createProject)
        {
            var result = await _marketingRespository.CreateProject(createProject);
            return new OkObjectResult($"{result} - Project Created in Service Application.");
        }
        public async Task<IActionResult> UpdateProject(int projectId, Request_UpdateProject updateProject)
        {
            var result = await _marketingRespository.UpdateProject(projectId, updateProject);
            return new OkObjectResult(result);
        }
        public async Task<IActionResult> UpdateBlocks(int projectId, Request_ProjectBlocks request)
        {
            var result = await _marketingRespository.UpdateBlocks(projectId, request);
            return new OkObjectResult(result);
        }
        public async Task<IActionResult> UpdateUnits(int projectId, Request_UnitsProjects request)
        {
            var result = await _marketingRespository.UpdateUnits(projectId, request);
            return new OkObjectResult(result);
        }
        public async Task<IActionResult> DeleteProject(int projectId, Request_DeleteProject deleteProject)
        {
            var result = await _marketingRespository.DeleteProject(projectId, deleteProject);
            return new OkObjectResult(result);
        }
        public async Task<IActionResult> ActivateProject(int projectId, Request_ActivateProject activateProject)
        {
            var result = await _marketingRespository.ActivateProject(projectId, activateProject);
            return new OkObjectResult(result);
        }
        public async Task<IActionResult> GetProjectById(int id)
        {
            var result = await _marketingRespository.GetProjectById(id);
            return new OkObjectResult(result);
        }
        public async Task<IActionResult> GetAllProjects()
        {
            var result = await _marketingRespository.GetAllProjects();
            return new OkObjectResult(result);
        }
        public async Task<ProjectFilesDTO> UploadProjectFiles([FromForm] ProjectFilesDTO projectFiles, IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("No file provided!");

            // Save file and metadata
            return await _marketingRespository.UploadProjectFiles(projectFiles, file);
        }
        public async Task<IActionResult> GetProjectImages(int projectId)
        {
            if (projectId == 0)
                throw new ArgumentException("No project Id found !");

            return await _marketingRespository.GetProjectImages(projectId);
        }

        #endregion

        #region Profile
        public async Task<IActionResult> CreateUserProfile(Request_CreateUserProfile userProfile)
        {
            var result = await _marketingRespository.CreateUserProfile(userProfile);
            if (result == null)
                return new BadRequestResult();
            return new OkObjectResult($"the User Profile {result} is created successfully !");
        }
        public async Task<IActionResult> UpdateUserProfile(int userProfileId, Request_UpdateUserProfile updateProfile)
        {
            var result = await _marketingRespository.UpdateUserProfile(userProfileId, updateProfile);
            if (result == null)
                return new BadRequestResult();
            return new NoContentResult();
        }
        public async Task<IActionResult> DeleteUserProfile(int userProfileId)
        {
            var result = await _marketingRespository.DeleteUserProfile(userProfileId);
            return new NoContentResult();
        }
        public async Task<IActionResult> GetAllUserProfiles()
        {
            var result = await _marketingRespository.GetAllUserProfiles();
            return new OkObjectResult("All results are found in DB !");
        }
        public async Task<IActionResult> GetUserProfile(int userProfileId)
        {
            var result = await _marketingRespository.GetUserProfile(userProfileId);
            if (result == null)
                return new NotFoundObjectResult($"the query to get the reusult {userProfileId} is not found any resord/s .");
            return new OkObjectResult($"User Profile founded : {result}");
        }
        #endregion
        
        #region Booking
        public async Task<IActionResult> CreateBooking(Request_CreateBooking createBooking)
        {
            var result = await _marketingRespository.CreateBooking(createBooking);
            return new OkObjectResult($"Booking created as : {result}");
        }
        public async Task<IActionResult> UpdateBooking(int updateBookingId ,Request_UpdateBooking updateBooking)
        {
            var result = await _marketingRespository.UpdateBooking(updateBookingId, updateBooking);
            if (result == null)
                throw new Exception("No record/s has been updated !");
            return new NoContentResult();
        }
        public async Task<IActionResult> DeleteBooking(int bookingId)
        {
            var result = await _marketingRespository.DeleteBooking(bookingId);
            return new NoContentResult();
        }
        public async Task<IActionResult> GetBookingById(int bookingId)
        {
            var result = await _marketingRespository.GetBookingById(bookingId);
            if (result == null)
                throw new Exception("No Booking/s found !");
            return new OkObjectResult(result);
        }
        public async Task<IActionResult> GetAllBookings()
        {
            var result = await _marketingRespository.GetAllBooking();
            if (result == null)
                throw new Exception("No Booking found ever !");
            return new OkObjectResult(result);
        }
        #endregion
    }
}
