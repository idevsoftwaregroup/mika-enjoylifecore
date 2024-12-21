namespace Messaging.Infrastructure.Services.DeliveryProviders.EmailDelivery;

public sealed class EmailSettings
{
    public const string SECTION_NAME = nameof(EmailSettings);
    public string FromAddress { get; set; }

    public string DefaultSubject { get; set; } = "Default Subject";

    public string SmtpServerAddress { get; set; }
    public int SmtpPort { get; set; }

    public string SmtpUsername { get; set; }
    public string SmtpPassword { get; set;}

}
