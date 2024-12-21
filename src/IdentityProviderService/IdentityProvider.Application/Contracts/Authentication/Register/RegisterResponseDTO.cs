namespace IdentityProvider.Application.Contracts.Authentication.Register;

public class RegisterResponseDTO
{
    public int Id { get; set; }
    public string PhoneNumber { get; set; }
    public string Username { get; set; }
    public string? Email { get; set; }
    public int CoreId { get; set; }
}
