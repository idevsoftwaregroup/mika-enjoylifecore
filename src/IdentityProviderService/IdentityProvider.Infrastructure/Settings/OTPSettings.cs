namespace IdentityProvider.Infrastructure.Settings;

public class OTPSettings
{
    public const string SECTION_NAME = nameof(OTPSettings);

    public string TextTemplate { get; set; } = null!;
    public string RequestURL { get; set; } = null!;
    public string RequestOTPUrl { get; set; }

}
