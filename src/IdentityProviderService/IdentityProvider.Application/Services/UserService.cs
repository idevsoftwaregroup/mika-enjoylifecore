using IdentityProvider.Application.Contracts.UserInfo;
using IdentityProvider.Application.Contracts.UserUpdate;
using IdentityProvider.Application.Interfaces;
using IdentityProvider.Application.Interfaces.Infrastructure;
using IdentityProvider.Domain.Models.EnjoyLifeUser;

namespace IdentityProvider.Application.Services;

public class UserService : IUserService
{
    private readonly IEnjoyLifeIdentityRepository _enjoyLifeIdentityRepository;

    public UserService(IEnjoyLifeIdentityRepository enjoyLifeIdentityRepository)
    {
        _enjoyLifeIdentityRepository = enjoyLifeIdentityRepository;
    }

    public async Task<UpdateUserResponseDTO> UpdateUserAsync(int userId, UpdateUserRequestDTO requestDTO, CancellationToken cancellationToken = default)
    {
        EnjoyLifeUser enjoyLifeUser = await _enjoyLifeIdentityRepository.GetUserById(userId, cancellationToken) ?? throw new Exception($"user with Id {userId} was not found");
        enjoyLifeUser.PhoneNumber = requestDTO.PhoneNumber;
        enjoyLifeUser.UserName = requestDTO.UserName;
        enjoyLifeUser.Email = requestDTO.Email;
        enjoyLifeUser.CoreId = requestDTO.CoreId;

        return await _enjoyLifeIdentityRepository.UpdateUserAsync(enjoyLifeUser, cancellationToken); // implicit cast is happening
    }

    public async Task DeleteUserAsync(int userId, CancellationToken cancellationToken = default)
    {
        EnjoyLifeUser user = await _enjoyLifeIdentityRepository.GetUserById(userId, cancellationToken) ?? throw new Exception($"user with Id {userId} was not found");
        await _enjoyLifeIdentityRepository.DeleteUserAsync(user);
    }

  
}
