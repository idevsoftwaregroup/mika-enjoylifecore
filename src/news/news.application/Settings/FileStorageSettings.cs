namespace news.application.Settings;

public class FileStorageSettings
{
    public const string SECTION_NAME = "FileStorage";

    public string RootDirectoryPath { get; set; }
    public List<string> ImageContentTypes { get; set; }
    public List<string> VideoContentTypes { get; set; }
    public List<string> GifContentTypes { get; set; }


}
