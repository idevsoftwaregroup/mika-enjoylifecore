namespace core.infrastructure.FileServices;

public class FileStorageSettings
{
    public const string SECTION_NAME = nameof(FileStorageSettings);

    public string FileStorageUrl { get; set; }
    public int TicketingServiceType { get; set; }
}
