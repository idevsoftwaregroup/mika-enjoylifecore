using Azure;
using core.application.Framework;
using core.domain.DomainModelDTOs.UserDTOs;
using core.domain.entity.structureModels;
using Microsoft.EntityFrameworkCore;

namespace core.application.Contract.Infrastructure
{
    public interface IUserRepository
    {
        Task<int> AddUserAsync(UserModel user);
        Task<int> DeleteUserAsync(int id);
        Task<List<UserModel>> GetAllUserAsync();
        Task<UserModel> GetUserAsync(int id);
        Task<UserModel> GetUserByPhoneAsync(string phoneNumber);
        Task<List<string>> GetUsersPhoneNumberAsync(int ComplexId);
        Task<int> UpdateUserAsync(UserModel user);
        Task<OperationResult<object>> SetUserConnection(int userId, string connection);
        Task<OperationResult<object>> RemoveUserConnection(int userId);
        Task<OperationResult<object>> RemoveUserConnection(string connection);
        Task<List<string>> GetAllConnections();
        Task<List<string>> GetOtherConnections(int userId);
        Task<OperationResult<Response_CreatedUserDomainDTO>> CreateUserAndResident(int adminId, Request_CreateUserAndResidentDomainDTO model, CancellationToken cancellationToken = default);
        Task<OperationResult<Response_CreatedUserDomainDTO>> CreateUserAndOwner(int adminId, Request_CreateUserAndOwnerDomainDTO model, CancellationToken cancellationToken = default);
        Task<OperationResult<Response_CreatedUserDomainDTO>> UpdateUserAndResident(int adminId, Request_UpdateUserAndResidentDomainDTO model, CancellationToken cancellationToken = default);
        Task<OperationResult<Response_CreatedUserDomainDTO>> UpdateUserAndOwner(int adminId, Request_UpdateUserAndOwnerDomainDTO model, CancellationToken cancellationToken = default);
        Task<OperationResult<object>> DeleteUserAndOwnerResident(int adminId, Request_DeleteUserDomainDTO model, CancellationToken cancellationToken = default);
        Task<OperationResult<object>> DeleteOwnership(Request_DeleteUserDomainDTO model, CancellationToken cancellationToken = default);
        Task<OperationResult<object>> DeleteResidence(Request_DeleteUserDomainDTO model, CancellationToken cancellationToken = default);
        Task<OperationResult<Response_GetUsersByUnitDomainDTO>> GetUsersByUnitId(Request_UnitIdDomainDTO model, CancellationToken cancellationToken = default);
        Task<OperationResult<Response_GetUserDomainDTO>> GetCoreUsers(Filter_GetUserDomainDTO? filter, CancellationToken cancellationToken = default);
        Task<OperationResult<Response_CreatedResidentOwnerOfUnitDmainDTO>> CreateResidentsOwnersOfUnit(int adminId, Request_CreateResidentsOwnersOfUnitDomainDTO model, CancellationToken cancellationToken = default);
        Task<OperationResult<Response_CreateOrModifiedUserIdDomainDTO>> CreateCoreUser(int adminId, Request_CreateUserDomainDTO model, CancellationToken cancellationToken = default);
        Task<OperationResult<Response_CreateOrModifiedUserIdDomainDTO>> UpdateCoreUser(int adminId, Request_UpdateUserDomainDTO model, CancellationToken cancellationToken = default);
        Task<OperationResult<object>> DeleteCoreUser(int adminId, Request_DeleteUserDomainDTO model, CancellationToken cancellationToken = default);

    }
}
