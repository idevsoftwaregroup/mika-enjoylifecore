using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using core.application.Framework;
using core.domain.DomainModelDTOs.LogDTOs.ActivityDTOs.FilterDTOs;
using core.domain.DomainModelDTOs.LogDTOs.ActivityDTOs;

namespace core.application.Contract.API.Interfaces
{
    public interface IActivityService
    {
        Task<OperationResult<object>> SetLog_Login(UserIdDTO model, CancellationToken cancellationToken = default);
        Task<OperationResult<object>> SetLog_Logout(UserIdDTO model, CancellationToken cancellationToken = default);
        Task<OperationResult<GetActivityLogDTO>> GetActivityLogs(Filter_GetActivityLogDTO filter, CancellationToken cancellationToken = default);
    }
}
