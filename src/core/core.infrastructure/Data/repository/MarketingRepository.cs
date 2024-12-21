using Azure.Core;
using core.application.Contract.API.DTO.Marketing;
using core.application.Contract.API.Interfaces;
using core.application.Contract.infrastructure;
using core.domain.DomainModelDTOs.MIKAMarketingDTOs;
using core.infrastructure.Data.persist;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.infrastructure.Data.repository
{
    public class MarketingRepository : IMarketingRepository
    {
        #region Constructors
        private readonly EnjoyLifeContext _context; // main model injector .
        // upload file path :
        private readonly string _uploadPath;
        public MarketingRepository(EnjoyLifeContext context)
        {
            _context = context;
        }
        public IActionResult CatchManager(Exception ex)
        {
            return new JsonResult(new
            {
                Message = "Profile Update has error on service .",
                Details = ex.Message
            })
            {
                StatusCode = StatusCodes.Status500InternalServerError, // Set approperiate status for Error 500 on Server.
                ContentType = "application/json",
            };
        }
        #endregion
        #region Project/s
        public async Task<IActionResult> CreateProject(Request_CreateProject projects)
        {
            if (projects == null)
            {
                return new BadRequestObjectResult("Invalid project data.");
            }

            try
            {
                // Validate mandatory fields
                if (string.IsNullOrEmpty(projects.ProjectName) || string.IsNullOrEmpty(projects.ProjectType))
                {
                    return new BadRequestObjectResult("Project name and type are required.");
                }

                var project = new MIKAMarketingProjectsDTO
                {
                    ProjectName = projects.ProjectName,
                    ProjectDescription = projects.ProjectDescription,
                    ProjectType = projects.ProjectType,
                    ProjectLocation = projects.ProjectLocation,
                    ProjectFloors = projects.ProjectFloors,
                    ProjectBlocks = projects.ProjectBlocks,
                    ProjectDateTimeDelivery = projects.ProjectDateTimeDelivery,
                    ProjectBasePrice = projects.ProjectBasePrice,
                    ProjectMultiplyProjectPerBlock = projects.ProjectMultiplyProjectPerBlock,
                    ProjectUnits = projects.ProjectUnits,
                    ProjectBuildingMeterages = projects.ProjectBuildingMeterages,
                    ProjectBlocksDetails = "",
                    ProjectUnitsDetails = "",
                    ProjectDateTimeRegistration = DateTime.Now,
                    IsDeleted = false,
                };

                // Save the new project into the database
                _context.MIKAMarketing_Projects.Add(project);
                await _context.SaveChangesAsync();

                return new OkObjectResult(project);
            }
            catch (Exception ex)
            {
                return new ObjectResult($"Error occurred: {ex.Message}") { StatusCode = 500 };
            }
        }

        public async Task<IActionResult> UpdateProject(int projectId, Request_UpdateProject request)
        {
            try
            {
                // پیدا کردن پروژه با استفاده از ProjectId
                var project = await _context.MIKAMarketing_Projects
                    .FirstOrDefaultAsync(p => p.Id == projectId);

                // اگر پروژه یافت نشد
                if (project == null)
                {
                    return new NotFoundObjectResult($"Project with ID {projectId} not found.");
                }

                // بروزرسانی ویژگی‌های پروژه
                project.ProjectName = request.ProjectName;
                project.ProjectDescription = request.ProjectDescription;
                project.ProjectType = request.ProjectType;
                project.ProjectLocation = request.ProjectLocation;
                project.ProjectFloors = request.ProjectFloor;
                project.ProjectBlocks = request.ProjectBlocks;
                project.ProjectBasePrice = request.ProjectBasePrice;
                project.ProjectDateTimeDelivery = request.ProjectDateTimeDelivery;
                project.ProjectMultiplyProjectPerBlock = request.ProjectMultiplyProjectPerBlock;
                project.ProjectUnits = request.ProjectUnits;
                

                // بروزرسانی لیست واحدها (اگر داده‌ای ارسال شده باشد)
                if (request.ProjectUnits != null)
                {
                }

                // بروزرسانی لیست متراژهای ساختمان
                if (request.ProjectBuildingMeterages != null)
                {
                    project.ProjectBuildingMeterages = request.ProjectBuildingMeterages;
                }

                // ذخیره تغییرات در پایگاه داده
                await _context.SaveChangesAsync();

                return new OkObjectResult(project);
            }
            catch (Exception ex)
            {
                return CatchManager(ex); // مدیریت خطاها
            }
        }
        public async Task<IActionResult> UpdateBlocks(int projectId, Request_ProjectBlocks request)
        {
            try
            {
                // Fetch the project based on the projectId
                var project = await _context.MIKAMarketing_Projects.FindAsync(projectId);
                if (project == null)
                {
                    return new NotFoundObjectResult("Project not found!");
                }

                // If ProjectBlocksDetails is null or empty, return a bad request
                if (request.ProjectBlocksDetails == null || !request.ProjectBlocksDetails.Any())
                {
                    return new BadRequestObjectResult("No block details provided to update.");
                }

                // Convert the list of ProjectBlockDetail to a JSON string
                var blocksJson = JsonConvert.SerializeObject(request.ProjectBlocksDetails); // Using Newtonsoft.Json

                // Update the ProjectBlocksDetails field with the JSON string
                project.ProjectBlocksDetails = blocksJson;

                // Save changes to the database
                await _context.SaveChangesAsync();

                // Return a 204 No Content response for success
                return new NoContentResult(); // Or Ok() for a success message if needed
            }
            catch (Exception ex)
            {
                // Log the exception and return a custom error handler result
                return CatchManager(ex); // Custom exception handling logic (CatchManager)
            }
        }

        public async Task<IActionResult> UpdateUnits(int projectId, Request_UnitsProjects request)
        {
            try
            {
                // Fetch the project based on the projectId
                var project = await _context.MIKAMarketing_Projects.FindAsync(projectId);
                if (project == null)
                {
                    return new NotFoundObjectResult("Project not found!");
                }

                // If ProjectBlocksDetails is null or empty, return a bad request
                if (request.ProjectUnitsDetails == null || !request.ProjectUnitsDetails.Any())
                {
                    return new BadRequestObjectResult("No block details provided to update.");
                }

                var blocksJson = JsonConvert.SerializeObject(request.ProjectUnitsDetails);  // Serialize the list to JSON

                project.ProjectUnitsDetails = blocksJson;  // Store the JSON string in the project entity

                await _context.SaveChangesAsync();  // Save changes



                // Return a 204 No Content response for success
                return new NoContentResult(); // Or Ok() for a success message if needed
            } catch (Exception ex)
            {
                // Log the exception and return a custom error handler result
                return CatchManager(ex); // Custom exception handling logic (CatchManager)
            }
        }

        public async Task<IActionResult> DeleteProject(int projectId, Request_DeleteProject deletedProject)
        {
            if (projectId == 0)
                return new BadRequestObjectResult("The Project is NOT Found!");

            try
            {
                var result = await _context.MIKAMarketing_Projects.FirstOrDefaultAsync(del => del.Id == projectId);

                if (result == null)
                {
                    return new NotFoundObjectResult($"Project with ID {projectId} not found.");
                }

                // Set the project as deleted
                result.IsDeleted = true;

                // Optionally, you can update other properties based on `deletedProject` if necessary
                // result.OtherProperty = deletedProject.SomeValue;

                // Save changes to the database
                await _context.SaveChangesAsync();

                return new OkObjectResult($"The Deactivation of this Project with #NO: {projectId} is Done!");
            }
            catch (Exception ex)
            {
                return CatchManager(ex); // Handle the exception and return a proper response
            }
        }

        public async Task<IActionResult> ActivateProject(int projectId, Request_ActivateProject activateProject)
        {
            if (projectId == 0)
                return new BadRequestObjectResult("The Project is NOT Found!");

            try
            {
                var result = await _context.MIKAMarketing_Projects.FirstOrDefaultAsync(activate => activate.Id == projectId);

                if (result == null)
                {
                    return new NotFoundObjectResult($"Project with ID {projectId} not found.");
                }

                // Set the project as deleted
                result.IsDeleted = false;

                // Optionally, you can update other properties based on `deletedProject` if necessary
                // result.OtherProperty = deletedProject.SomeValue;

                // Save changes to the database
                await _context.SaveChangesAsync();

                return new OkObjectResult($"The Deactivation of this Project with #NO: {projectId} is Done!");
            } catch (Exception ex)
            {
                return CatchManager(ex);
            }
        }

        public async Task<IActionResult> GetProjectById(int id)
        {
            try
            {
                var result = await _context.MIKAMarketing_Projects
                                           .FirstOrDefaultAsync(finder => finder.Id == id);

                if (result == null)
                {
                    return new NotFoundResult(); // Return 404 if no project is found.
                }

                //var response = new
                //{
                //    data = result,
                //    count = result.Count
                //};

                return new JsonResult(result); // Returning the entire project object
            }
            catch (Exception ex)
            {
                return CatchManager(ex);
            }

        }
        public async Task<IActionResult> GetAllProjects()
        {
            try
            {
                var result = await _context.MIKAMarketing_Projects
                    .Select(finder => new MIKAMarketingProjectsDTO
                    {
                        Id = finder.Id,
                        ProjectBasePrice = finder.ProjectBasePrice,
                        ProjectName = finder.ProjectName,
                        ProjectBlocks = finder.ProjectBlocks,
                        ProjectBuildingMeterages = finder.ProjectBuildingMeterages,
                        ProjectDateTimeDelivery = finder.ProjectDateTimeDelivery,
                        ProjectDateTimeRegistration = finder.ProjectDateTimeRegistration,
                        ProjectDescription = finder.ProjectDescription,
                        ProjectFloors = finder.ProjectFloors,
                        ProjectUnitsDetails = finder.ProjectUnitsDetails,
                        ProjectType = finder.ProjectType,
                        ProjectLocation = finder.ProjectLocation,
                        ProjectMultiplyProjectPerBlock = finder.ProjectMultiplyProjectPerBlock,
                        ProjectUnits = finder.ProjectUnits,
                        IsDeleted = finder.IsDeleted,
                    })
                    .ToListAsync();

                var response = new
                {
                    data = result,
                    count = result.Count
                };

                return new JsonResult(result);  // Returning the result directly
            }
            catch (Exception ex)
            {
                return CatchManager(ex);
            }
        }
        public async Task<ProjectFilesDTO> UploadProjectFiles([FromForm] ProjectFilesDTO projectFiles, IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Invalid file.");

            // Set up directory
            if (!Directory.Exists("/home/mika/api/fileserver/FileStorage/Marketing/Vardavard"))
                Directory.CreateDirectory("/home/mika/api/fileserver/FileStorage/Marketing/Vardavard");

            // Generate unique filename and path
            var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var fullFilePath = Path.Combine("/home/mika/api/fileserver/FileStorage/Marketing/Vardavard", uniqueFileName);

            // Save the file to the server
            await using (var stream = new FileStream(fullFilePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Save metadata to DB (adjust the URL structure to match the web URL)
            var fileMetadata = new ProjectFilesDTO
            {
                FileName = file.FileName,
                ProjectId = projectFiles.ProjectId,
                FileDescription = projectFiles.FileDescription,
                // The URL should reflect the public file path structure
                FileUrl = $"https://app.enjoylife.ir/filestorage/MikaFiles/Marketing/Vardavard/{uniqueFileName}",
                UploadedDate = DateTime.UtcNow
            };

            await _context.MIKAMarketing_ProjectFiles.AddAsync(fileMetadata);
            await _context.SaveChangesAsync();

            return fileMetadata;

        }
        public async Task<IActionResult> GetProjectImages(int projectId)
        {
            if (projectId == 0)
                throw new ArgumentException("No related project Id found !");

            try
            {
                // Find the project with the given projectId in the MIAMarketing_ProjectFiles table
                var projectExists = await _context.MIKAMarketing_ProjectFiles
                                                  .FirstOrDefaultAsync(persist => persist.ProjectId == projectId);

                if (projectExists == null)
                    throw new ArgumentException("Project not found!");

                // Fetch images related to the projectId
                var result = await _context.MIKAMarketing_ProjectFiles
                                           .Where(project => project.ProjectId == projectId)
                                           .Select(project => new Response_GetProjectImages
                                           {
                                               ProjectId = project.ProjectId,
                                               FileDescription = project.FileDescription,
                                               FileName = project.FileName,
                                               FileUrl = project.FileUrl,
                                               UploadedDate = project.UploadedDate
                                           })
                                           .ToListAsync();

                // If no images are found, return a default message
                if (result.Count == 0)
                {
                    return new JsonResult(new { message = "No images found for this project." });
                }

                var data = new
                {
                    result = result
                };

                return new JsonResult(data);

            }
            catch (Exception ex)
            {
                return CatchManager(ex);
            }

        }
        #endregion
        #region Profile
        public async Task<IActionResult> CreateUserProfile(Request_CreateUserProfile createUserProfile)
        {
            try
            {
                var result = new MIKAMarketingUserProfile
                {
                    Id = createUserProfile.Id,
                    UserFullName = createUserProfile.UserFullName,
                    UserNationalId = createUserProfile.UserNationalId,
                    UserPhoneNumber = createUserProfile.UserPhoneNumber,
                    UserEmail = createUserProfile.UserEmail,
                    UserJobTitle = createUserProfile.UserJobTitle,
                    UserDateOfBirth = createUserProfile.UserDateOfBirth,
                };
                await _context.MIKAMarketing_UserProfiles.AddRangeAsync(result);
                await _context.SaveChangesAsync();
                return new OkObjectResult($"{result}");
            } catch (Exception ex)
            {
                return CatchManager(ex);
            }
        }
        public async Task<IActionResult> UpdateUserProfile(int useId, Request_UpdateUserProfile updateUserProfile)
        {
            try
            {
                var result = await _context.MIKAMarketing_UserProfiles.FirstOrDefaultAsync(x => x.Id == useId);
                if (result != null)
                {
                    result.UserFullName = updateUserProfile.UserFullName;
                    result.UserNationalId = updateUserProfile.UserNationalId;
                    result.UserJobTitle = updateUserProfile.UserJobTitle;
                    result.UserPhoneNumber = updateUserProfile.UserPhoneNumber;
                    result.UserEmail = updateUserProfile.UserEmail;
                    result.UserDateOfBirth = updateUserProfile.UserDateOfBirth;
                    await _context.SaveChangesAsync();
                }
                return new OkObjectResult($"{result}");
            } catch (Exception ex)
            {
                return CatchManager(ex);
            }
        }
        public async Task<IActionResult> DeleteUserProfile(int userId)
        {
            try
            {
                var result = await _context.MIKAMarketing_UserProfiles.FirstOrDefaultAsync(x => x.Id ==  userId);
                _context.MIKAMarketing_UserProfiles.Remove(result);
                await _context.SaveChangesAsync();
                return new OkObjectResult($"{result}");
            } catch (Exception ex)
            {
                return CatchManager(ex);
            }
        }
        public async Task<IActionResult> GetUserProfile(int userId)
        {
            try
            {
                var result = await _context.MIKAMarketing_UserProfiles.FirstOrDefaultAsync(x => x.Id == userId);
                return new OkObjectResult($"{result}");
            } catch (Exception ex)
            {
                return CatchManager(ex);
            }
        }
        public async Task<IActionResult> GetAllUserProfiles()
        {
            try
            {
                var result = await _context.MIKAMarketing_UserProfiles.ToListAsync();
                return new OkObjectResult($"{result}");
            } catch (Exception ex)
            {
                return CatchManager(ex);
            }
        }
        #endregion
        #region Booking
        public async Task<IActionResult> CreateBooking(Request_CreateBooking createBooking)
        {
            try
            {
                var newBooking = new MIKAMarketingBooking
                {
                    Id = createBooking.Id,
                    ProjectId = createBooking.ProjectId,
                    UserProfileId = createBooking.UserProfileId,
                    BookingTotalPrice = createBooking.BookingTotalPrice,
                    BookingBlock = createBooking.BookingBlock,
                    BookingFloor = createBooking.BookingFloor,
                    BookingUnits = createBooking.BookingUnits,
                    BookingInstallmentAccounting = createBooking.BookingInstallmentAccounting,
                    BookingPrepaidPrecentPrice = createBooking.BookingPrepaidPrecentPrice,
                    BookingDateTimeRegistration = DateTime.Now.ToString(),
                };

                await _context.MIKAMarketing_Bookings.AddAsync(newBooking);
                await _context.SaveChangesAsync();

                return new OkObjectResult(newBooking);
            }
            catch (Exception ex)
            {
                return CatchManager(ex);
            }
        }
        public async Task<IActionResult> UpdateBooking(int bookingId, Request_UpdateBooking updateBooking)
        {
            try
            {
                // Fetch the booking by ID
                var result = await _context.MIKAMarketing_Bookings
                    .Include(b => b.UserProfileId)  // Include related entities if needed
                    .Include(b => b.ProjectId)
                    .FirstOrDefaultAsync(c => c.Id == bookingId);

                // Check if the booking exists
                if (result != null)
                {
                    // Update the booking fields with the new values
                    result.UserProfileId = updateBooking.UserProfileId;
                    result.ProjectId = updateBooking.ProjectId;
                    result.BookingTotalPrice = updateBooking.BookingTotalPrice;
                    result.BookingPrepaidPrecentPrice = updateBooking.BookingPrepaidPrecentPrice;
                    result.BookingDateUpdated = DateTime.Now.ToString();
                    result.BookingInstallmentAccounting = updateBooking.BookingInstallmentAccounting;

                    // Optionally update related entities (if necessary)
                    if (updateBooking.UserProfileId != null)
                        result.UserProfileId = updateBooking.UserProfileId;
                    if (updateBooking.ProjectId != null)
                        result.ProjectId = updateBooking.ProjectId;

                    // Save changes
                    await _context.SaveChangesAsync();

                    // Return the updated booking result
                    return new OkObjectResult($"{result}");
                }
                else
                {
                    return new NotFoundObjectResult($"Booking with ID {bookingId} not found.");
                }
            }
            catch (Exception ex)
            {
                // Log exception and return a managed response
                return CatchManager(ex);
            }
        }
        public async Task<IActionResult> DeleteBooking(int bookingId)
        {
            try
            {
                var result = await _context.MIKAMarketing_Bookings.FirstOrDefaultAsync(m => m.Id == bookingId);
                if (result != null)
                {
                    _context.MIKAMarketing_Bookings.Remove(result);
                    await _context.SaveChangesAsync();
                }
                return new OkObjectResult($"{result}");
            } catch (Exception ex)
            {
                return CatchManager(ex);
            }
        }
        public async Task<IActionResult> GetBookingById(int bookingId)
        {
            try
            {
                var result = await _context.MIKAMarketing_Bookings.FirstOrDefaultAsync(x => x.Id == bookingId);
                if (result == null)
                {
                    return new NotFoundObjectResult("No result found !");
                }
                return new OkObjectResult($"{result}");
            } catch (Exception ex)
            {
                return CatchManager(ex);
            }
        }

        public async Task<IActionResult> GetAllBooking()
        {
            try
            {
                var result = await _context.MIKAMarketing_Bookings.ToListAsync();
                return new OkObjectResult($"{result}");
            } catch (Exception ex)
            {
                return CatchManager(ex);
            }
        }
        #endregion
    }
}
