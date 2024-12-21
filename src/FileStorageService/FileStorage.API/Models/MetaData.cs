namespace FileStorage.API.Models;

public class MetaData
{
    public static readonly List<string> ValidContentTypes = new List<string>() { "image/jpeg", "image/png", "video/mp4", "video/mkv", "video/avi", "application/pdf", "audio/mpeg" };
    public static readonly List<string> ValidFileExtensions = new List<string>() { ".jpg", ".jpeg" , ".png", ".mp4", ".mkv", ".avi",".pdf",".mp3" };
    public static readonly List<string> ValidFinanceContentTypes = new List<string>() { "image/jpeg", "image/png", "application/pdf" };
    public static readonly List<string> ValidFinanceFileExtensions = new List<string>() { ".jpg", ".jpeg", ".png", ".pdf" };

    //implement this as a map from content type to file extension

    public Guid Id { get; set; }
    public string OriginName { get; set; } = null!;
    public ServiceType Service { get; set; }
    public Uri Path { get; set; } = null!;
    public string FilePath { get; set; } = null!;
    //public OriginService OriginService { get; set; } = null!;
    public string ContentType { get; set; } = null!;
    public long ByteSize { get; set; }
    public DateTime CreateDate { get; set; }
    public string CreateBy { get; set; } = null!;
    public bool IsDeleted { get; set; }
}
