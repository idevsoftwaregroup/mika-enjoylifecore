using OrganizationChart.API.Contracts.DTOs;
using OrganizationChart.API.Framework;
using OrganizationChart.API.Models;

namespace OrganizationChart.API.Contracts.Interfaces;

public interface ISystemDetails
{
    Task<OperationResult<GetSystemTypeDTO>> GetSystemTypes(CancellationToken cancellationToken = default);
    Task<OperationResult<GetSystemDTO>> GetSystem(string computerName, CancellationToken cancellationToken = default);
    Task<OperationResult<object>> CreateSystem(CreateSystemDTO model, CancellationToken cancellationToken = default);
    Task<OperationResult<List<GetSystemDTO>>> GetAllSystems(CancellationToken cancellationToken = default);
    Task<OperationResult<GetSystemDTO>> UpdateSystem(UpdateSystemDetailsDTO model, CancellationToken cancellationToken = default);
    Task<OperationResult<object>> DeleteSystem(SystemIdDTO requestDTO, CancellationToken cancellationToken = default);




}

