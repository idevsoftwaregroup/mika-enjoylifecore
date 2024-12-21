using core.application.Contract.API.DTO.Marketing;
using core.domain.DomainModelDTOs.MIKAMarketingDTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.application.Contract.infrastructure
{
    public interface IMarketingRepository
    {
        #region Project/s
        Task<IActionResult> CreateProject([FromBody] Request_CreateProject mikaProjects);
        Task<IActionResult> UpdateProject(int projectId, Request_UpdateProject updateProject);
        Task<IActionResult> UpdateBlocks(int projectId, Request_ProjectBlocks request);
        Task<IActionResult> UpdateUnits(int projectId, Request_UnitsProjects request);
        Task<IActionResult> DeleteProject(int projectId, Request_DeleteProject deleteProject);
        Task<IActionResult> ActivateProject(int projectId, Request_ActivateProject activateProject);
        Task<IActionResult> GetProjectById(int id);
        Task<IActionResult> GetAllProjects();
        Task<ProjectFilesDTO> UploadProjectFiles([FromForm] ProjectFilesDTO projectFiles, IFormFile file);
        Task<IActionResult> GetProjectImages(int projectId);
        #endregion
        #region Profile
        Task<IActionResult> CreateUserProfile(Request_CreateUserProfile createUserProfile);
        Task<IActionResult> UpdateUserProfile(int userId, Request_UpdateUserProfile updateUserProfile);
        Task<IActionResult> DeleteUserProfile(int userId);
        Task<IActionResult> GetUserProfile(int userId);
        Task<IActionResult> GetAllUserProfiles();
        #endregion
        #region Booking
        Task<IActionResult> CreateBooking(Request_CreateBooking createUserProfile);
        Task<IActionResult> UpdateBooking(int bookingId, Request_UpdateBooking updateUserProfile);
        Task<IActionResult> DeleteBooking(int bookingId);
        Task<IActionResult> GetBookingById(int bookingId);
        Task<IActionResult> GetAllBooking();
        #endregion
    }
}
