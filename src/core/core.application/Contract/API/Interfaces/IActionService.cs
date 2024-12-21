using core.domain.DomainModelDTOs.LogDTOs.ActionDTOs.FilterDTOs;
using core.domain.DomainModelDTOs.LogDTOs.ActionDTOs;
using core.domain.DomainModelDTOs.LogDTOs.ModuleDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using core.application.Framework;

namespace core.application.Contract.API.Interfaces
{
    public interface IActionService
    {
        Task<OperationResult<GetModuleDTO>> GetModules(CancellationToken cancellationToken = default);
        Task<OperationResult<object>> SetLog_Action(int userId, SetLogDTO model, CancellationToken cancellationToken = default);
        Task<OperationResult<GetActionLogDTO>> GetActionLogs(Filter_GetActionLogDTO filter, CancellationToken cancellationToken = default);
    }
}
