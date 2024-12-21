namespace IdentityProvider.Application.Contracts.UserUpdate;

public class UpdateUserRequestDTO
{
    public string UserName { get; set; }
    public string PhoneNumber { get; set; }
    public string? Email { get; set; }
    public int CoreId { get; set; }
}
