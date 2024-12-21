namespace FileStorage.API.Contracts;

public class DownloadRequestDTO
{
    public int ServiceTypeNumber { get; set; }
    public string OwnerId { get; set; } = null!;
    //public string OriginName { get; set; } = null!;
    public string? Position { get; set; }

}
