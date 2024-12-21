using IdentityProvider.Application.Framework;
using IdentityProvider.Domain.DomainModelDTOs;
using IdentityProvider.Domain.DomainModelDTOs.Users;
using IdentityProvider.Domain.Models.EnjoyLifeUser;
using System.Security.Claims;

namespace IdentityProvider.Application.Interfaces.Infrastructure;

public interface IEnjoyLifeIdentityRepository
{
    bool UserExistsByPhoneNumber(string phoneNumber);
    Task<EnjoyLifeUser?> GetUserByPhoneNumber(string phoneNumber, CancellationToken cancellationToken);
    Task SeedAdmin();
    Task<IEnumerable<Claim>> GetUserClaimsAsync(EnjoyLifeUser user);
    Task<EnjoyLifeUser> CreateUserAsync(EnjoyLifeUser enjoyLifeUser, CancellationToken cancellationToken);
    Task<EnjoyLifeUser> UpdateUserAsync(EnjoyLifeUser user, CancellationToken cancellationToken = default);
    Task<EnjoyLifeUser?> GetUserById(int Id, CancellationToken cancellationToken = default);
    Task DeleteUserAsync(EnjoyLifeUser user);
    Task<OperationResult<object>> CreateUser(Request_CreateUserDomainDTO model, CancellationToken cancellationToken = default);
    Task<OperationResult<object>> CreateUsers(List<Request_CreateUserDomainDTO> model, CancellationToken cancellationToken = default);
    Task<OperationResult<object>> UpdateUser(Request_UpdateUserDomainDTO model, CancellationToken cancellationToken = default);
    Task<OperationResult<object>> DeleteUser(Request_DeleteUserDomainDTO model, CancellationToken cancellationToken = default);
    Task<OperationResult<object>> SendOTPEmail(string emailOrPhoneNumber, string otpValue, CancellationToken cancellationToken = default);
    Task<OperationResult<EnjoyLifeUser>> LoginByPassword(string emailOrPhoneNumber, string password, CancellationToken cancellationToken = default);
    Task<OperationResult<EnjoyLifeUser>> CheckUserForLoginByOTP(string emailOrPhoneNumber, CancellationToken cancellationToken = default);
    Task<OperationResult<EnjoyLifeUser>> GetUserEmailOrPhoneNumber(string emailOrPhoneNumber, CancellationToken cancellationToken = default);
    Task<OperationResult<EnjoyLifeUser>> LoginLobbyMan(int CoreId, CancellationToken cancellationToken = default);
    Task<OperationResult<object>> CreateOrUpdateIdentityUser(Request_CreateOrUpdateIdentityUserDomainDTO model, CancellationToken cancellationToken = default);
    Task<OperationResult<object>> DeleteIdentityUser(Request_CoreId model, CancellationToken cancellationToken = default);
}
