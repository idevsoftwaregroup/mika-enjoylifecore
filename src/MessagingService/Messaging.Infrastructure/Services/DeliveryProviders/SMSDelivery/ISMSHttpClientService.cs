using Messaging.Infrastructure.Services.DeliveryProviders.SMSDelivery.Contracts;

namespace Messaging.Infrastructure.Services.DeliveryProviders.SMSDelivery
{
    public interface ISMSHttpClientService
    {
        Task<SendSMSResponse> SendBasicSMSAsync(SendSMSRequest request, CancellationToken cancellationToken);
        Task<SendSMSResponse> SendBulkInstanceSMSAsync(SendSMSRequest request, CancellationToken cancellationToken);
    }
}