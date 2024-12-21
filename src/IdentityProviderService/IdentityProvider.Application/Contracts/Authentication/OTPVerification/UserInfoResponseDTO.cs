namespace IdentityProvider.Application.Contracts.Authentication.OTPVerification;

public class UserInfoResponseDTO
{
    public int UserId { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public string Token { get; set; }
    public bool IsLobbyAttendant { get; set; }
}
