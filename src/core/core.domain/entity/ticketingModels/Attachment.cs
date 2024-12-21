namespace core.domain.entity.ticketingModels;

public class Attachment
{
    public long Id { get; set; }
    public string Url { get; set; }

    public Attachment(string url)
    {
        
        Url = url;
    }
}
