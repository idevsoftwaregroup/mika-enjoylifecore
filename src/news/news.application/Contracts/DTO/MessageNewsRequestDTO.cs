namespace news.application.Contracts.DTO;

public class MessageNewsRequestDTO
{
    //public string NewsArticleId { get; set; } 
    public string NewsArticleTitle { get; set; }
    public List<string> PhoneNumbers { get; set; } = new();
}
