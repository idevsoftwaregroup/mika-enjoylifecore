using Microsoft.AspNetCore.Identity;

namespace IdentityProvider.Domain.Models.EnjoyLifeUser;

public class EnjoyLifeUser : IdentityUser<int>
{
    public int CoreId { get; set; }
}
