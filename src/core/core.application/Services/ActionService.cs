using core.application.Contract.API.Interfaces;
using core.application.Contract.infrastructure;
using core.domain.DomainModelDTOs.LogDTOs.ActionDTOs.FilterDTOs;
using core.domain.DomainModelDTOs.LogDTOs.ActionDTOs;
using core.domain.DomainModelDTOs.LogDTOs.ModuleDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using core.application.Framework;

namespace core.application.Services
{
    public class ActionService : IActionService
    {
        private readonly IActionRepository _actionRepository;
        public ActionService(IActionRepository actionRepository)
        {
            _actionRepository = actionRepository;
        }

        public async Task<OperationResult<GetActionLogDTO>> GetActionLogs(Filter_GetActionLogDTO filter, CancellationToken cancellationToken = default)
        {
            return await _actionRepository.GetActionLogs(filter, cancellationToken);
        }

        public async Task<OperationResult<GetModuleDTO>> GetModules(CancellationToken cancellationToken = default)
        {
            return await _actionRepository.GetModules(cancellationToken);
        }

        public async Task<OperationResult<object>> SetLog_Action(int userId, SetLogDTO model, CancellationToken cancellationToken = default)
        {
            return await _actionRepository.SetLog_Action(userId, model, cancellationToken);
        }
    }
}
