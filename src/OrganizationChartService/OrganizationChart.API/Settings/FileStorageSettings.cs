namespace OrganizationChart.API.Settings;

public class FileStorageSettings
{
    public const string SECTION_NAME = nameof(FileStorageSettings);

    public string FileStorageUrl { get; set; }
    public List<string> ValidFileContentTypes { get; set; }
    public List<string> ValidFileExtensions { get; set; }
    public int ServiceType { get; set; }
}
