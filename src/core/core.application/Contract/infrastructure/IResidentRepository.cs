using core.application.Contract.API.DTO.Party.Resident;
using core.domain.entity.partyModels;

namespace core.application.Contract.Infrastructure
{
    public interface IResidentRepository
    {
        Task<int> CreateResidentAsync(ResidentCreateRequest residentCreateRequest);
        List<ResidentModel> GetResidents(ResidentGetRequestFilter filter);
        Task<int> UpdateResidentAsync(ResidentUpdateRequest residentCreateRequest);
        Task<int?> GetUnitIdByUser(int userId);

    }
}
