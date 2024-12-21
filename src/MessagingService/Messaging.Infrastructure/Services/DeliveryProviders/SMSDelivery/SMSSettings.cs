namespace Messaging.Infrastructure.Services.DeliveryProviders.SMSDelivery;

public class SMSSettings
{
    public const string SECTION_NAME = nameof(SMSSettings);

    public string BaseURL { get; set; } = null!;
    public string APIKey { get; set; } = null!;
    public string BasicSendURL { get; set; } = null!;
    public string SenderLine { get; set; } = null!;
    public string ReceptorQueryParamName { get; set; } = null!;
    public string TextQueryParamName { get; set; } = null!;
    public string SenderLineQueryParamName { get; set; } = null!;
    
}
