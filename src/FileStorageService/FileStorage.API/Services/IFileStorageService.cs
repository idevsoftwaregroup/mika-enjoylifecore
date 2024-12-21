using FileStorage.API.Contracts;

namespace FileStorage.API.Services
{
    public interface IFileStorageService
    {
        //Task<List<string>> GetFile(DownloadRequestDTO requestDTO, CancellationToken cancellationToken);
        Task<UploadResponseDTO> SaveFile(MemoryStream memoryStream, UploadMetaDataRequestDTO requestDTO, string fileName, string contentType, CancellationToken cancellationToken = default);
        Task<UploadResponseDTO> SaveFinanceFile(MemoryStream memoryStream, string fileName, string contentType, CancellationToken cancellationToken);
    }
}