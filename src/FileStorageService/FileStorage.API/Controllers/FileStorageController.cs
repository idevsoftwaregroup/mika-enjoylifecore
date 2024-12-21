using FileStorage.API.Contracts;
using FileStorage.API.Models;
using FileStorage.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Net.Mime;

namespace FileStorage.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileStorageController : ControllerBase
    {
        private readonly IFileStorageService _fileStorageService;

        public FileStorageController(IFileStorageService fileStorageService)
        {
            _fileStorageService = fileStorageService;
        }

        [HttpPost("UploadFinance")]
        public async Task<ActionResult<UploadResponseDTO>> UploadFinanceFile(IFormFile formFile,CancellationToken cancellationToken = default)
        {
            if (formFile == null)
            {
                return BadRequest("No file uploaded.");
            }


            using var memoryStream = new MemoryStream();

            
            await formFile.CopyToAsync(memoryStream, cancellationToken);
            string fileName = formFile.FileName;
            string contentType = formFile.ContentType;//.ToLowerInvariant();
 
            return Ok(await _fileStorageService.SaveFinanceFile(memoryStream, fileName, contentType, cancellationToken));

        }

        [HttpPost("Upload")]
        public async Task<ActionResult<UploadResponseDTO>> UploadFile(IFormFile formFile, [FromQuery] UploadMetaDataRequestDTO request, CancellationToken cancellationToken = default) 
        {
            if (formFile == null)
            {
                return BadRequest("No file uploaded.");
            }
            //check its not finance
            if (request.ServiceType == ServiceType.Finance) return BadRequest("cant save finance file through this path");



            using var memoryStream = new MemoryStream();
            await formFile.CopyToAsync(memoryStream, cancellationToken);
            string fileName = formFile.FileName;
            string contentType = formFile.ContentType.ToLower();//.ToLowerInvariant();


            return Ok(await _fileStorageService.SaveFile(memoryStream, request, fileName, contentType, cancellationToken));

        }

        //[HttpGet("Download",Name = "Download")]
        //public async Task<ActionResult<List<string>>> DownloadFile([FromQuery] DownloadRequestDTO request ,CancellationToken cancellationToken)
        //{
        //    return Ok(await _fileStorageService.GetFile(request,cancellationToken));
        //}
    }
}