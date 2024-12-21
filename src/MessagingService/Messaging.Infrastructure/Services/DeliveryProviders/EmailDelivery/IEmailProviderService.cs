using Messaging.Infrastructure.Services.DeliveryProviders.EmailDelivery.Contracts;

namespace Messaging.Infrastructure.Services.DeliveryProviders.EmailDelivery;

public interface IEmailProviderService
{
    Task<SendEmailResponse> SendEmail(SendEmailRequest request, CancellationToken cancellationToken);
}
