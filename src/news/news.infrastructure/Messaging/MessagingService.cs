using news.application.Contracts.Interfaces;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using news.application.Contracts.DTO;

namespace news.infrastructure.Messaging;

public class MessagingService : IMessagingService
{
    private readonly HttpClient _httpClient;
    private readonly MessagingSettings _messagingSettings;
    private readonly ILogger<MessagingService> _logger;
    public MessagingService(HttpClient httpClient, IOptions<MessagingSettings> options, ILogger<MessagingService> logger)
    {
        _httpClient = httpClient;
        _messagingSettings = options.Value;
        _logger = logger;
    }

    public async Task SendNewsViaSMS(MessageNewsRequestDTO dto, CancellationToken cancellationToken = default)
    {
        try
        {
            Dictionary<string, string> replacements = new Dictionary<string, string>();

           // replacements.Add(nameof(dto.NewsArticleId), dto.NewsArticleId);
            replacements.Add(nameof(dto.NewsArticleTitle), dto.NewsArticleTitle);

            string text = Regex.Replace(_messagingSettings.NewsUserTemplate, @"\{(.+?)\}", m => replacements[m.Groups[1].Value]);
           
            List<string> phoneNumbers = new List<string>(_messagingSettings.NewsSMSGroup);
            phoneNumbers.AddRange(dto.PhoneNumbers);
            await SendGroupSMS(phoneNumbers, text, cancellationToken);
        }
        catch (Exception eee)
        {

            var ff = eee.Message;
        }

    }

    public async Task SendGroupSMS(List<string> phoneNumbers, string text, CancellationToken cancellationToken = default)
    {
        var body = new
        {
            Text = text,
            PhoneNumbers = phoneNumbers,
            DeliveryMethodType = 0
        };

        string json = JsonSerializer.Serialize(body);


        HttpContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");

        HttpResponseMessage response = await _httpClient.PostAsync(_messagingSettings.BaseURL, httpContent, cancellationToken);

        if (!response.IsSuccessStatusCode) throw new Exception("group sms was not sent");

        var responseBody = JsonSerializer.Deserialize<BulkSMSResponse>(await response.Content.ReadAsStringAsync()) ?? throw new Exception("response could not be deserialized");

        if (!response.IsSuccessStatusCode) throw new Exception($"group sms was not sent for {responseBody.Status}"); // error handling needs some work
        //_logger.LogError(response.);
    }

    private class BulkSMSResponse
    {
        public Dictionary<string, bool> Status { get; set; }
    }
}
