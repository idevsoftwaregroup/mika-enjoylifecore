using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using core.application.Contract.API.Interfaces;
using core.application.Contract.infrastructure;
using core.application.Framework;
using core.domain.DomainModelDTOs.LogDTOs.ActivityDTOs.FilterDTOs;
using core.domain.DomainModelDTOs.LogDTOs.ActivityDTOs;

namespace core.application.Services
{
    public class ActivityService : IActivityService
    {
        private readonly IActivityRepository _activityRepository;
        public ActivityService(IActivityRepository activityRepository)
        {
            _activityRepository = activityRepository;
        }

        public async Task<OperationResult<GetActivityLogDTO>> GetActivityLogs(Filter_GetActivityLogDTO filter, CancellationToken cancellationToken = default)
        {
            return await _activityRepository.GetActivityLogs(filter, cancellationToken);
        }

        public async Task<OperationResult<object>> SetLog_Login(UserIdDTO model, CancellationToken cancellationToken = default)
        {
            return await _activityRepository.SetLog_Login(model, cancellationToken);
        }

        public async Task<OperationResult<object>> SetLog_Logout(UserIdDTO model, CancellationToken cancellationToken = default)
        {
            return await _activityRepository.SetLog_Logout(model, cancellationToken);
        }
    }
}
