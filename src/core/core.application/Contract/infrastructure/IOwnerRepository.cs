using core.application.Contract.API.DTO.Party.Owner;
using core.domain.entity.partyModels;

namespace core.application.Contract.Infrastructure;

public interface IOwnerRepository
{

    Task<int> AddAsync(OwnerCreateRequest ownerCreateDTO);
    List<OwnerModel> GetOwners(OwnerGetRequestFilter filter);
    Task<int> UpdateAsync(OwnerUpdateRequest ownerModel);
}
