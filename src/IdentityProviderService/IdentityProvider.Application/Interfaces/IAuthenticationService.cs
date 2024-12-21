using IdentityProvider.Application.Contracts.Authentication.Login;
using IdentityProvider.Application.Contracts.Authentication.OTPVerification;
using IdentityProvider.Application.Contracts.Authentication.Register;
using IdentityProvider.Application.Framework;
using IdentityProvider.Domain.DomainModelDTOs;
using IdentityProvider.Domain.DomainModelDTOs.Users;

namespace IdentityProvider.Application.Interfaces
{
    public interface IAuthenticationService
    {
        Task<RegisterResponseDTO> RegisterUser(RegisterRequestDTO requestDTO, CancellationToken cancellationToken);
        Task SeedAdmin();
        Task<OperationResult<object>> CreateUser(Request_CreateUserDomainDTO model, CancellationToken cancellationToken = default);
        Task<OperationResult<object>> CreateUsers(List<Request_CreateUserDomainDTO> model, CancellationToken cancellationToken = default);
        Task<OperationResult<object>> UpdateUser(Request_UpdateUserDomainDTO model, CancellationToken cancellationToken = default);
        Task<OperationResult<object>> DeleteUser(Request_DeleteUserDomainDTO model, CancellationToken cancellationToken = default);
        Task<OperationResult<UserInfoResponseDTO>> LoginByPassword(Request_LoginByPasswrodDomainDTO model, CancellationToken cancellationToken = default);
        Task<OperationResult<object>> LoginByOTP(LoginRequestDTO requestDTO, CancellationToken cancellationToken);
        Task<OperationResult<UserInfoResponseDTO>> VerifyOTP(OTPVerificationRequestDTO requestDTO, CancellationToken cancellationToken);
        Task<OperationResult<UserInfoResponseDTO>> LoginLobbyMan(int CoreId, CancellationToken cancellationToken = default);
        Task<OperationResult<object>> CreateOrUpdateIdentityUser(Request_CreateOrUpdateIdentityUserDomainDTO model, CancellationToken cancellationToken = default);
        Task<OperationResult<object>> DeleteIdentityUser(Request_CoreId model, CancellationToken cancellationToken = default);
    }
}