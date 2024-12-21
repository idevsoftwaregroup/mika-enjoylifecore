using OrganizationChart.API.Contracts.DTOs;
using OrganizationChart.API.Framework;

namespace OrganizationChart.API.Contracts.Interfaces
{
    public interface ISystemUpsRepository
    {
        Task<OperationResult<GetSystemProjectDTO>> GetUpsProjects(CancellationToken cancellationToken = default);
        Task<OperationResult<GetUpsDTO>> GetUPSes(CancellationToken cancellationToken = default);
        Task<OperationResult<GetUpsDTO>> GetUPS(int Id, CancellationToken cancellationToken = default);
        Task<OperationResult<object>> CreateUps(CreateUpsDTO model, CancellationToken cancellationToken = default);
        Task<OperationResult<object>> UpdateUps(UpdateUpsDTO model, CancellationToken cancellationToken = default);
        Task<OperationResult<object>> DeleteUps(DeleteUpsDTO model, CancellationToken cancellationToken = default);
        Task<OperationResult<object>> CreateUpsService(CreateUpsServiceDTO model, CancellationToken cancellationToken = default);
        Task<OperationResult<object>> UpdateUpsService(UpdateUpsServiceDTO model, CancellationToken cancellationToken = default);
        Task<OperationResult<object>> DeleteUpsService(DeleteUpsServiceDTO model, CancellationToken cancellationToken = default);
    }
}
