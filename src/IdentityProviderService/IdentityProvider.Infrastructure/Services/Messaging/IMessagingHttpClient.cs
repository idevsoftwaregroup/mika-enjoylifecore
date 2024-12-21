namespace IdentityProvider.Infrastructure.Services.Messaging
{
    public interface IMessagingHttpClient
    {
        Task SendOTPSMS(string recipient, string text, CancellationToken cancellationToken);
    }
}