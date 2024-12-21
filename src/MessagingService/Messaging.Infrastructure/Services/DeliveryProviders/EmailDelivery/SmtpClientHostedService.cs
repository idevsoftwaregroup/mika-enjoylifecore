using MailKit.Net.Smtp;
using Microsoft.Extensions.Hosting;

namespace Messaging.Infrastructure.Services.DeliveryProviders.EmailDelivery;

internal class SmtpClientHostedService : IHostedService
{
    private readonly SmtpClient _smtpClient;

    public SmtpClientHostedService(SmtpClient smtpClient)
    {
        _smtpClient = smtpClient;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        // No action needed here, just return a completed task.
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        // Dispose of the SmtpClient when the application is stopping.

        //_smtpClient.Disconnect(true);

        _smtpClient.Dispose();
        return Task.CompletedTask;
    }
}
