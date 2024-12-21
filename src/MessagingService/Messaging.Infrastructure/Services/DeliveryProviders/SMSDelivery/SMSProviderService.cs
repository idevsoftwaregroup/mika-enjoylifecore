using Messaging.Infrastructure.Services.DeliveryProviders.SMSDelivery.Contracts;

namespace Messaging.Infrastructure.Services.DeliveryProviders.SMSDelivery;

public class SMSProviderService : ISMSProviderService
{
    private readonly ISMSHttpClientService _httpClientService;

    public SMSProviderService(ISMSHttpClientService httpClientService)
    {
        _httpClientService = httpClientService;
    }


    public async Task<SendSMSResponse> SendSMS(SendSMSRequest request,CancellationToken cancellationToken)
    {
        return await _httpClientService.SendBasicSMSAsync(request,cancellationToken);
    }

    public async Task<SendSMSResponse> SendBulkInstanceSMS(SendSMSRequest request, CancellationToken cancellationToken)
    {
        return await _httpClientService.SendBulkInstanceSMSAsync(request, cancellationToken);
    }
}
