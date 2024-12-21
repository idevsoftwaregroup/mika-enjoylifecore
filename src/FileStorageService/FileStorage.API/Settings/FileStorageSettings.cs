namespace FileStorage.API.Settings;

public class FileStorageSettings
{
    public const string SECTION_NAME = nameof(FileStorageSettings);
    public string RootPath { get; set; }
    public string UserRootPath { get; set; }    
}
