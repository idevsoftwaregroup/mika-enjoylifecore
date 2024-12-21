using Messaging.Infrastructure.Services.DeliveryProviders.SMSDelivery.Contracts;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Options;

namespace Messaging.Infrastructure.Services.DeliveryProviders.SMSDelivery;

public class SMSHttpClientService : ISMSHttpClientService
{
    private readonly SMSSettings _settings;
    private readonly HttpClient _httpClient;

    public SMSHttpClientService(IOptions<SMSSettings> options, HttpClient httpClient)
    {
        _settings = options.Value;
        _httpClient = httpClient;
    }

    public async Task<SendSMSResponse> SendBasicSMSAsync(SendSMSRequest request, CancellationToken cancellationToken)
    {
        //var response = await _httpClient.GetAsync($"{_settings.BaseURL}/{_settings.APIKey}/{_settings.BasicSendURL}?" +
        //    $"{_settings.ReceptorQueryParamName}={request.Receptor}" +
        //    $"&{_settings.SenderLineQueryParamName}={_settings.SenderLine}"+
        //    $"&{_settings.TextQueryParamName}={request.Text}", cancellationToken);


        var tokenaddress = $"https://api.kavenegar.com/v1/{_settings.APIKey}/verify/lookup.json?receptor={request.Receptor}&token={request.Text}&template=enjoylifelogin";
        var response = await _httpClient.GetAsync(tokenaddress, cancellationToken);


        return new SendSMSResponse(response.IsSuccessStatusCode);

    }

    public async Task<SendSMSResponse> SendBulkInstanceSMSAsync(SendSMSRequest request, CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetAsync($"{_settings.BaseURL}/{_settings.APIKey}/{_settings.BasicSendURL}?" +
            $"{_settings.ReceptorQueryParamName}={request.Receptor}" +
            $"&{_settings.SenderLineQueryParamName}={_settings.SenderLine}" +
            $"&{_settings.TextQueryParamName}={request.Text}", cancellationToken);

        return new SendSMSResponse(response.IsSuccessStatusCode);
    }
}
