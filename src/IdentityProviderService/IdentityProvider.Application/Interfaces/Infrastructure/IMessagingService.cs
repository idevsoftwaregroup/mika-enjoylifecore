namespace IdentityProvider.Application.Interfaces.Infrastructure;

public interface IMessagingService
{
    Task SendOTPMessage(string phoneNumber, string otpValue, CancellationToken cancellationToken);
}
