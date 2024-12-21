using IdentityProvider.Infrastructure.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using static System.Net.WebRequestMethods;

namespace IdentityProvider.Infrastructure.Services.Messaging;

public class MessagingHttpClient : IMessagingHttpClient
{
    private readonly ILogger<MessagingHttpClient> _logger;
    private readonly HttpClient _httpClient;
    private readonly OTPSettings _settings;
    public MessagingHttpClient(HttpClient httpClient, IOptions<OTPSettings> options, ILogger<MessagingHttpClient> logger)
    {
        _httpClient = httpClient;
        _settings = options.Value;
        _logger = logger;
    }

    public async Task SendOTPSMS(string recipient, string Body, CancellationToken cancellationToken) // check the needed format and try to make it not hardcoded so if messaging service needed a different format we dont have to recompile maybe
    {
        //var data = new Dictionary<string, string>
        //    {
        //        { $"{nameof(recipient)}", $"{recipient}" },
        //        {"DeliveryMethod","0" },
        //        { $"{nameof(Body)}", $"{Body}" },
        //        {"Sender","IdentityProvider" }

        //    };

        //object obj = new { Recipient = recipient, DeliveryMethod = 0, Body, Sender = "IdentityProvider" };
        string json = JsonSerializer.Serialize(new
        {
            Phonenumber = recipient,
            OTP = Body
        });


        HttpContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");

        HttpResponseMessage response = await _httpClient.PostAsync(_settings.RequestOTPUrl, httpContent, cancellationToken);

        if (!response.IsSuccessStatusCode) _logger.LogError(await response.Content.ReadAsStringAsync()); // customize this to throw exception 

    }
}
