using core.domain.DomainModelDTOs.LogDTOs.ActivityDTOs.FilterDTOs;
using core.domain.DomainModelDTOs.LogDTOs.ActivityDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using core.application.Framework;

namespace core.application.Contract.infrastructure
{
    public interface IActivityRepository
    {
        Task<OperationResult<object>> SetLog_Login(UserIdDTO model, CancellationToken cancellationToken = default);
        Task<OperationResult<object>> SetLog_Logout(UserIdDTO model, CancellationToken cancellationToken = default);
        Task<OperationResult<GetActivityLogDTO>> GetActivityLogs(Filter_GetActivityLogDTO filter, CancellationToken cancellationToken = default);
    }
}
