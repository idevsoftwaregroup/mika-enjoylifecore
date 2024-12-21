namespace core.application.Contract.infrastructure
{
    public interface IFileStorageService
    {
        Task<string> UploadTicketingAttachment(Stream stream, string fileName, string contentType, int ticketId, long messageId, CancellationToken cancellationToken = default);
    }
}