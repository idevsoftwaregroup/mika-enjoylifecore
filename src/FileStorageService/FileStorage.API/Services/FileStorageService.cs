using FileStorage.API.Contracts;
using FileStorage.API.Infrastructure;
using FileStorage.API.Models;
using FileStorage.API.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace FileStorage.API.Services;

public class FileStorageService : IFileStorageService
{
    private readonly FileStorageSettings _settings;
    private readonly FileStorageDataContext _context;
    private readonly IWebHostEnvironment _hostingEnvironment;

    public FileStorageService(IOptions<FileStorageSettings> settings, FileStorageDataContext context, IWebHostEnvironment hostingEnvironment)
    {
        _settings = settings.Value;
        _context = context;
        _hostingEnvironment = hostingEnvironment;
    }

    public async Task<UploadResponseDTO> SaveFile(MemoryStream memoryStream, UploadMetaDataRequestDTO requestDTO, string fileName, string contentType, CancellationToken cancellationToken = default)
    {
        if (memoryStream == null || memoryStream.Length == 0)
        {
            throw new ArgumentException("MemoryStream is empty or null.");
        }

        ////check valid enum number
        //if ((int)requestDTO.ServiceType > Enum.GetValues<ServiceType>().Length) throw new Exception("invalid service type");
        
        //sanitize file name
        
        char[] invalidChars = Path.GetInvalidFileNameChars();
        
        if (fileName.IndexOfAny(invalidChars) >= 0)
        {
            throw new Exception("Invalid characters found in the file name.");
        }


        //check content type and file extension
        if (!Path.HasExtension(fileName) || !MetaData.ValidFileExtensions.Contains(Path.GetExtension(fileName).ToLower())) throw new Exception("file extension is invalid");
        if (!MetaData.ValidContentTypes.Contains(contentType.ToLower())) throw new Exception("invalid content type");

        MetaData metaData = new()
        {
            Id = Guid.NewGuid(),
            ByteSize = memoryStream.Length,
            CreateDate = DateTime.Now,
            ContentType = contentType,
            OriginName = fileName,
            Service = requestDTO.ServiceType,
            IsDeleted = false,
            CreateBy = "unkown",

        };


        await CreateFile(metaData,memoryStream,cancellationToken);

        await _context.MetaDatas.AddAsync(metaData);
        await _context.SaveChangesAsync();

        return new UploadResponseDTO() {  URL = metaData.FilePath };
    }

    public async Task<UploadResponseDTO> SaveFinanceFile(MemoryStream memoryStream, string fileName, string contentType, CancellationToken cancellationToken)
    {
        if (memoryStream == null || memoryStream.Length == 0)
        {
            throw new ArgumentException("MemoryStream is empty or null.");
        }

        //sanitize file name

        char[] invalidChars = Path.GetInvalidFileNameChars();

        if (fileName.IndexOfAny(invalidChars) >= 0)
        {
            throw new Exception("Invalid characters found in the file name.");
        }

        //check content type and file extension
        if (!Path.HasExtension(fileName) || !MetaData.ValidFinanceFileExtensions.Contains(Path.GetExtension(fileName).ToLower())) throw new Exception("file extension is invalid");
        if (!MetaData.ValidFinanceContentTypes.Contains(contentType.ToLower())) throw new Exception("invalid content type");

        MetaData metaData = new()
        {
            Id = Guid.NewGuid(),
            ByteSize = memoryStream.Length,
            CreateDate = DateTime.Now,
            ContentType = contentType,
            OriginName = fileName,
            Service = ServiceType.Finance,
            IsDeleted = false,
            CreateBy = "unkown",

        };

        await CreateFile(metaData, memoryStream, cancellationToken);

        await _context.MetaDatas.AddAsync(metaData);
        await _context.SaveChangesAsync();

        return new UploadResponseDTO() { URL = metaData.FilePath };

    }

    private async Task CreateFile(MetaData metaData, MemoryStream memoryStream, CancellationToken cancellationToken = default)
    {
        var serverFilePath = _settings.RootPath;
        var userFilePath = _settings.UserRootPath;
        string relativePath = Path.Combine(metaData.Service.ToString(), metaData.CreateDate.ToString("yyyy-MM-dd"));
        string fileName = metaData.Id.ToString("N") + Path.GetExtension(metaData.OriginName);
        string wwwrootPath = _hostingEnvironment.ContentRootPath;
        string filePath = Path.Combine(wwwrootPath,serverFilePath, relativePath, fileName);

        if (filePath.IndexOfAny(Path.GetInvalidPathChars()) >= 0) throw new Exception("file path has invalid chars");
        metaData.Path = new Uri(filePath);
        metaData.FilePath = Path.Combine(userFilePath, relativePath, fileName).Replace('\\', '/');
      
        Directory.CreateDirectory(Path.GetDirectoryName(filePath) ?? throw new Exception("directory could not be created"));
        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            memoryStream.Seek(0, SeekOrigin.Begin);
            await memoryStream.CopyToAsync(fileStream, cancellationToken);
        }
    }

}
