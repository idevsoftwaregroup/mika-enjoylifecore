namespace Messaging.Infrastructure.Services.DeliveryProviders.SMSDelivery.Contracts;

public record SendSMSRequest(string Receptor,string Text, string? Sender = null);
