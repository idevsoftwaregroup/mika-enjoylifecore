using core.application.Contract.API.DTO.LobbyAttendant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using core.application.Framework;
using core.application.Contract.API.DTO.LobbyAttendant.Filter;

namespace core.application.Contract.API.Interfaces
{
    public interface ILobbyAttendantService
    {
        Task<OperationResult<Reponse_GetLobbyAttendantDTO>> GetLobbyAttendants(Filter_GetLobbyAttendantDTO filter, CancellationToken cancellationToken = default);
        Task<OperationResult<object>> CreateLobbyAttendant(int AdminId, Request_CreateLobbyAttendantDTO model, CancellationToken cancellationToken = default);
        Task<OperationResult<object>> UpdateLobbyAttendant(int AdminId, Request_UpdateLobbyAttendantDTO model, CancellationToken cancellationToken = default);
        Task<OperationResult<object>> DeleteLobbyAttendant(Request_LobbyAttendantIdDTO model, CancellationToken cancellationToken = default);
        Task<OperationResult<object>> LoginLobbyAttendant(Request_LoginLobbyAttendantDTO model, CancellationToken cancellationToken = default);
        Task<OperationResult<Response_LobbyAttendantUnitsDTO>> GetUnitsForLobbyAttendant(Request_LoginLobbyAttendantDTO model, CancellationToken cancellationToken = default);
        Task<OperationResult<Response_LobbyAttendantUnitsDTO>> GetUnitsForLobbyAttendant_Authorized(CancellationToken cancellationToken = default);
        Task<OperationResult<Response_LobbyAttendantUsersInUnitDTO>> GetUsersInUnitForLobbyAttendant(int UnitId, Request_LoginLobbyAttendantDTO model, CancellationToken cancellationToken = default);
        Task<OperationResult<Response_LobbyAttendantUsersInUnitDTO>> GetUsersInUnitForLobbyAttendant_Authorized(int UnitId, CancellationToken cancellationToken = default);
        Task<OperationResult<Response_LoginLobbyAttendantDTO>> SecondLoginLobbyAttendant(Request_LobbyAttendandGetTokenDTO model, CancellationToken cancellationToken = default);
        Task<OperationResult<Response_LoginLobbyAttendantDTO>> SecondLoginLobbyAttendant_Authorized(Request_LobbyAttendantUserIdDTO model, CancellationToken cancellationToken = default);
    }
}
