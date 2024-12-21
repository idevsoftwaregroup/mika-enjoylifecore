using core.application.Contract.API.DTO.Party.Resident;
using core.application.Contract.API.Interfaces;
using core.application.Contract.API.Mapper;
using core.application.Contract.Infrastructure;

namespace core.application.Services;

public class ResidentService : IResidentService
{
    public IResidentRepository _residentRepository { get; set; }
    public ResidentService(IResidentRepository residentRepository)
    {
        _residentRepository = residentRepository;
    }
    public async Task<IEnumerable<ResidentGetResponse>> GetAllResidentsAsync(ResidentGetRequestFilter filter)
    {
        var result = _residentRepository.GetResidents(filter)
                .Select(x => x.ConvertResidentModelTOResidentGetResponse())
                .ToList();
        return result;

    }
    public async Task<int> CreateResident(ResidentCreateRequest residentCreateRequest)
    {
        int result = await _residentRepository.CreateResidentAsync(residentCreateRequest);
        return result;
    }
    public async Task<bool> UpdateResident(ResidentUpdateRequest residentCreateRequest)
    {
        return await _residentRepository.UpdateResidentAsync(residentCreateRequest) > 0;

    }
}
