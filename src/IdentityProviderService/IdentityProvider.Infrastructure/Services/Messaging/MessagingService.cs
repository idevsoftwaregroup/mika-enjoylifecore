using IdentityProvider.Application.Interfaces.Infrastructure;
using IdentityProvider.Infrastructure.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace IdentityProvider.Infrastructure.Services.Messaging;

public class MessagingService : IMessagingService
{
    private readonly ILogger<MessagingService> _logger;
    private readonly OTPSettings _settings;
    private readonly IMessagingHttpClient _httpClient;

    public MessagingService(ILogger<MessagingService> logger, IOptions<OTPSettings> options, IMessagingHttpClient httpClient)
    {
        _logger = logger;
        _settings = options.Value;
        _httpClient = httpClient;
    }

    public async Task SendOTPMessage(string phoneNumber, string otpValue, CancellationToken cancellationToken)
    {
        string template = _settings.TextTemplate;
        string text = template.Replace("{otpvalue}", otpValue);
        await _httpClient.SendOTPSMS(phoneNumber, otpValue, cancellationToken);
    }
}
