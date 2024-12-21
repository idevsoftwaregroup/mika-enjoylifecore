namespace OrganizationChart.API.Infrastructure.FileStorage
{
    public interface IFileStorageService
    {
        Task<string> SendProfilePictureToFileStorage(Stream stream, string fileName, string contentType, int employeeId, CancellationToken cancellationToken = default);
    }
}