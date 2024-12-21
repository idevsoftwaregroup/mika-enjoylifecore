using MailKit.Net.Smtp;
using Messaging.Infrastructure.Services.DeliveryProviders.EmailDelivery.Contracts;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;

namespace Messaging.Infrastructure.Services.DeliveryProviders.EmailDelivery;

public class EmailProviderService : IEmailProviderService
{
    private readonly ILogger<EmailProviderService> _logger;
    private readonly EmailSettings _emailSettings;
    private readonly SmtpClient _smtpClient;

    private static readonly object _emailLock = new object();
    public EmailProviderService(IOptions<EmailSettings> emailSettingsOptions, ILogger<EmailProviderService> logger, SmtpClient smtpClient)
    {
        _emailSettings = emailSettingsOptions.Value;
        _logger = logger;
        _smtpClient = smtpClient;
    }

    public async Task<SendEmailResponse> SendEmail(SendEmailRequest request, CancellationToken cancellationToken)
    {
        var email = new MimeMessage();
        try
        {

            email.From.Add(MailboxAddress.Parse(request.FromAddress ?? _emailSettings.FromAddress));
            email.To.Add(MailboxAddress.Parse(request.ToAddress));

        }
        catch (ParseException ex)
        {
            _logger.LogError(ex, "email address could not be parsed");
            return new SendEmailResponse(false, "email address could not be parsed");
        }

        email.Subject = request.Subject ?? _emailSettings.DefaultSubject;
        email.Body = new TextPart(TextFormat.Plain)
        {
            Text = request.Body,
        };


        //await _smtpClient.AuthenticateAsync(_emailSettings.SmtpUsername, _emailSettings.SmtpPassword, cancellationToken);
        // maybe authentication should be done once in configuration or maybe the client needs continuous authentication

        await Task.Run(() =>
        {
            lock (_emailLock)
            {
                _smtpClient.Send(email,cancellationToken);
            }
        },cancellationToken);

        return new SendEmailResponse(true);

    }
}
