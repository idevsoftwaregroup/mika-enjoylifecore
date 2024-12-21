namespace news.infrastructure.Messaging;

public class MessagingSettings
{
    public const string SECTION_NAME = nameof(MessagingSettings);
    public List<string> NewsSMSGroup { get; set; }
    public string BaseURL { get; set; }
    public string NewsUserTemplate { get; set; }
}
