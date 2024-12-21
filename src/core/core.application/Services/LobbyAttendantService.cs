using core.application.Contract.API.DTO.LobbyAttendant;
using core.application.Contract.API.DTO.LobbyAttendant.Filter;
using core.application.Contract.API.Interfaces;
using core.application.Contract.infrastructure;
using core.application.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.application.Services
{
    public class LobbyAttendantService : ILobbyAttendantService
    {
        private readonly ILobbyAttendantRepostory _lobbyAttendantRepostory;
        public LobbyAttendantService(ILobbyAttendantRepostory lobbyAttendantRepostory)
        {
            this._lobbyAttendantRepostory = lobbyAttendantRepostory;
        }
        public async Task<OperationResult<object>> CreateLobbyAttendant(int AdminId, Request_CreateLobbyAttendantDTO model, CancellationToken cancellationToken = default)
        {
            return await _lobbyAttendantRepostory.CreateLobbyAttendant(AdminId, model, cancellationToken);
        }

        public async Task<OperationResult<object>> DeleteLobbyAttendant(Request_LobbyAttendantIdDTO model, CancellationToken cancellationToken = default)
        {
            return await _lobbyAttendantRepostory.DeleteLobbyAttendant(model, cancellationToken);
        }

        public async Task<OperationResult<Reponse_GetLobbyAttendantDTO>> GetLobbyAttendants(Filter_GetLobbyAttendantDTO filter, CancellationToken cancellationToken = default)
        {
            return await _lobbyAttendantRepostory.GetLobbyAttendants(filter, cancellationToken);
        }

        public async Task<OperationResult<Response_LobbyAttendantUnitsDTO>> GetUnitsForLobbyAttendant(Request_LoginLobbyAttendantDTO model, CancellationToken cancellationToken = default)
        {
            return await _lobbyAttendantRepostory.GetUnitsForLobbyAttendant(model, cancellationToken);
        }

        public async Task<OperationResult<Response_LobbyAttendantUnitsDTO>> GetUnitsForLobbyAttendant_Authorized(CancellationToken cancellationToken = default)
        {
            return await _lobbyAttendantRepostory.GetUnitsForLobbyAttendant_Authorized(cancellationToken);
        }

        public async Task<OperationResult<Response_LobbyAttendantUsersInUnitDTO>> GetUsersInUnitForLobbyAttendant(int UnitId, Request_LoginLobbyAttendantDTO model, CancellationToken cancellationToken = default)
        {
            return await _lobbyAttendantRepostory.GetUsersInUnitForLobbyAttendant(UnitId, model, cancellationToken);
        }

        public async Task<OperationResult<Response_LobbyAttendantUsersInUnitDTO>> GetUsersInUnitForLobbyAttendant_Authorized(int UnitId, CancellationToken cancellationToken = default)
        {
            return await _lobbyAttendantRepostory.GetUsersInUnitForLobbyAttendant_Authorized(UnitId, cancellationToken);
        }

        public async Task<OperationResult<object>> LoginLobbyAttendant(Request_LoginLobbyAttendantDTO model, CancellationToken cancellationToken = default)
        {
            return await _lobbyAttendantRepostory.LoginLobbyAttendant(model, cancellationToken);
        }

        public async Task<OperationResult<Response_LoginLobbyAttendantDTO>> SecondLoginLobbyAttendant(Request_LobbyAttendandGetTokenDTO model, CancellationToken cancellationToken = default)
        {
            return await _lobbyAttendantRepostory.SecondLoginLobbyAttendant(model, cancellationToken);
        }

        public async Task<OperationResult<Response_LoginLobbyAttendantDTO>> SecondLoginLobbyAttendant_Authorized(Request_LobbyAttendantUserIdDTO model, CancellationToken cancellationToken = default)
        {
            return await _lobbyAttendantRepostory.SecondLoginLobbyAttendant_Authorized(model, cancellationToken);
        }

        public async Task<OperationResult<object>> UpdateLobbyAttendant(int AdminId, Request_UpdateLobbyAttendantDTO model, CancellationToken cancellationToken = default)
        {
            return await _lobbyAttendantRepostory.UpdateLobbyAttendant(AdminId, model, cancellationToken);
        }
    }
}
