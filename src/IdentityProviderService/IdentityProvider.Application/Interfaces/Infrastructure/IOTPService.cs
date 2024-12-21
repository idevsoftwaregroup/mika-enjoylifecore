namespace IdentityProvider.Application.Interfaces.Infrastructure;

public interface IOTPService
{
    string GenerateOTP(string PhoneNumber);
    bool ValidateOTP(string PhoneNumber, string OTPValue);
}
