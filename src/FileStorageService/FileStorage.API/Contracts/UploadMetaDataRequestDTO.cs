using FileStorage.API.Models;

namespace FileStorage.API.Contracts;

public record UploadMetaDataRequestDTO
{
    public ServiceType ServiceType { get; set; }
}
