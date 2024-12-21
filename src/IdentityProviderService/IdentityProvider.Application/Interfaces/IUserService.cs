using IdentityProvider.Application.Contracts.UserInfo;
using IdentityProvider.Application.Contracts.UserUpdate;

namespace IdentityProvider.Application.Interfaces
{
    public interface IUserService
    {
        Task DeleteUserAsync(int userId, CancellationToken cancellationToken = default);
        Task<UpdateUserResponseDTO> UpdateUserAsync(int userId, UpdateUserRequestDTO requestDTO, CancellationToken cancellationToken = default);
    }
}