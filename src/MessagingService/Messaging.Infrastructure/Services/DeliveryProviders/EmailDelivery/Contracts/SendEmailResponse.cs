namespace Messaging.Infrastructure.Services.DeliveryProviders.EmailDelivery.Contracts;

public record SendEmailResponse(bool Sent, string? Message = null);
