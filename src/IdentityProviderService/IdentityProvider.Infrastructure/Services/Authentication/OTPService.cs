using IdentityProvider.Application.Interfaces.Infrastructure;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text.Json;

namespace IdentityProvider.Infrastructure.Services.Authentication;

public class OTPService : IOTPService
{
    private static readonly List<OTP> _otps = new();

    private readonly ILogger<OTPService> _logger;
    private readonly TimeSpan _timeout = TimeSpan.FromMinutes(5);
    public OTPService(ILogger<OTPService> logger)
    {
        _logger = logger;
        
    }

    public string GenerateOTP(string phoneNumber)
    {
        _logger.LogInformation($"otps: {JsonSerializer.Serialize(_otps)}");

        OTP otp = new OTP(phoneNumber);
        _otps.Add(otp); // check if this phonenumber already has an otp 

        _logger.LogInformation($"otp value created key:{otp.Key} value:{otp.Value}");
        _logger.LogInformation($"otps: {JsonSerializer.Serialize(_otps)}");

        return otp.Value;
    }


    public bool ValidateOTP(string PhoneNumber , string OTPValue)
    {
        lock (_otps)
        {
            _logger.LogInformation($"otps: {JsonSerializer.Serialize(_otps)}");

            OTP? otp = _otps.LastOrDefault(otp => otp.Key == PhoneNumber);

            if (otp == null)
            {
                return false;
            }
            if (otp.Value != OTPValue || otp.Timestamp - DateTime.Now > _timeout)
            {
                if (otp.Timestamp - DateTime.Now > _timeout) _otps.Remove(otp);  // probably need a background service to remove expired otps

                return false;

            }
            else
            {

                _otps.Remove(otp);
                _logger.LogInformation($"otp vlaue {otp.Key} was valid");

                return true;
            }
        }
    }

    private class OTP
    {
        public DateTime Timestamp { get; }
        public string Key { get; set; }
        public string Value { get; }
        public OTP(string key)
        {
            Key = key;
            Timestamp = DateTime.Now;

            using (var rng = RandomNumberGenerator.Create())
            {
                var bytes = new byte[4];
                rng.GetBytes(bytes);
                int otpValue = (BitConverter.ToInt32(bytes, 0) & int.MaxValue )% 10000;
                Value = otpValue.ToString("D4");
            }
        }
    }
}
