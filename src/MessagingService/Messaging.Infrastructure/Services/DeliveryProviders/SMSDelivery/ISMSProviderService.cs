using Messaging.Infrastructure.Services.DeliveryProviders.SMSDelivery.Contracts;

namespace Messaging.Infrastructure.Services.DeliveryProviders.SMSDelivery;

public interface ISMSProviderService
{
    Task<SendSMSResponse> SendBulkInstanceSMS(SendSMSRequest request, CancellationToken cancellationToken);
    Task<SendSMSResponse> SendSMS(SendSMSRequest request, CancellationToken cancellationToken);
}
