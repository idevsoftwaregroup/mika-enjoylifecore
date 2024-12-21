using core.application.Contract.API.DTO.Party.Resident;

namespace core.application.Contract.API.Interfaces;

public interface IResidentService
{
    Task<int> CreateResident(ResidentCreateRequest residentCreateRequest);
    Task<IEnumerable<ResidentGetResponse>> GetAllResidentsAsync(ResidentGetRequestFilter filter);
    Task<bool> UpdateResident(ResidentUpdateRequest residentCreateRequest);
}
