using System.ComponentModel.DataAnnotations;

namespace IdentityProvider.Application.Contracts.Authentication.Register;

public class RegisterRequestDTO
{
    [Phone]
    public string PhoneNumber { get; set; }
    public string? Username { get; set; }
    public string? Email { get; set; }
    public int CoreId { get; set; }
}
