namespace Messaging.Infrastructure.Services.DeliveryProviders.EmailDelivery.Contracts;

public record SendEmailRequest(string ToAddress , string? FromAddress, string Body , string? Subject);
