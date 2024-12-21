using IdentityProvider.Domain.Models.EnjoyLifeUser;

namespace IdentityProvider.Application.Contracts.UserUpdate;

public class UpdateUserResponseDTO
{
    public int Id { get; set; }
    public string UserName { get; set; }
    public string PhoneNumber { get; set; }
    public string? Email { get; set; }
    public int CoreId { get; set; }

    public static implicit operator UpdateUserResponseDTO(EnjoyLifeUser v)
    {
        return new UpdateUserResponseDTO()
        {
            Id = v.Id,
            CoreId = v.CoreId,
            Email = v.Email,
            PhoneNumber = v.PhoneNumber,
            UserName = v.UserName,
        };
    }
}
