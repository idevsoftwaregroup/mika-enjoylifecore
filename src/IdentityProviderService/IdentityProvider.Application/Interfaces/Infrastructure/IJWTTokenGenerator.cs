using IdentityProvider.Domain.Models.EnjoyLifeUser;

namespace IdentityProvider.Application.Interfaces.Infrastructure;

public interface IJWTTokenGenerator
{
    Task<string> GenerateTokenAsync(EnjoyLifeUser user);
}
