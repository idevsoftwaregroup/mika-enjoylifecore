using core.application.Contract.API.DTO.Party.User;
using core.application.Framework;
using core.domain.DomainModelDTOs.UserDTOs;

namespace core.application.Contract.API.Interfaces;

public interface IUserService
{
    Task<int> CreateUser(UserCreateRequest userCreateDTO);
    Task<List<UserGetResponseDTO>> GetAllUser();
    Task<UserGetResponseDTO> GetUserById(int id);
    Task<UserGetResponseDTO> GetUserByPhone(string phoneNumber);
    Task<List<string>> GetUsersPhoneNumber(int ComplexId);
    Task<bool> UpdateUser(UserUpdateRequest userUpdateDTO);
    Task<OperationResult<object>> SetUserConnection(int userId, string connection);
    Task<OperationResult<object>> RemoveUserConnection(int userId);
    Task<OperationResult<object>> RemoveUserConnection(string connection);
    Task<List<string>> GetAllConnections();
    Task<OperationResult<object>> CreateUserAndResident(string token, int adminId, Request_CreateUserAndResidentDomainDTO model, CancellationToken cancellationToken = default);
    Task<OperationResult<object>> CreateUserAndOwner(string token, int adminId, Request_CreateUserAndOwnerDomainDTO model, CancellationToken cancellationToken = default);
    Task<OperationResult<object>> UpdateUserAndResident(string token, int adminId, Request_UpdateUserAndResidentDomainDTO model, CancellationToken cancellationToken = default);
    Task<OperationResult<object>> UpdateUserAndOwner(string token, int adminId, Request_UpdateUserAndOwnerDomainDTO model, CancellationToken cancellationToken = default);
    Task<OperationResult<object>> DeleteUserAndOwnerResident(string token, int adminId, Request_DeleteUserDomainDTO model, CancellationToken cancellationToken = default);
    Task<OperationResult<object>> DeleteOwnership(Request_DeleteUserDomainDTO model, CancellationToken cancellationToken = default);
    Task<OperationResult<object>> DeleteResidence(Request_DeleteUserDomainDTO model, CancellationToken cancellationToken = default);
    Task<OperationResult<Response_GetUsersByUnitDomainDTO>> GetUsersByUnitId(Request_UnitIdDomainDTO model, CancellationToken cancellationToken = default);
    Task<OperationResult<object>> CreateResidentsOwnersOfUnit(string token, int adminId, Request_CreateResidentsOwnersOfUnitDomainDTO model, CancellationToken cancellationToken = default);
    Task<OperationResult<Response_GetUserDomainDTO>> GetCoreUsers(Filter_GetUserDomainDTO? filter, CancellationToken cancellationToken = default);
    Task<OperationResult<Response_CreateOrModifiedUserIdDomainDTO>> CreateCoreUser(int adminId, Request_CreateUserDomainDTO model, CancellationToken cancellationToken = default);
    Task<OperationResult<Response_CreateOrModifiedUserIdDomainDTO>> UpdateCoreUser(int adminId, Request_UpdateUserDomainDTO model, CancellationToken cancellationToken = default);
    Task<OperationResult<object>> DeleteCoreUser(int adminId, Request_DeleteUserDomainDTO model, CancellationToken cancellationToken = default);
}
