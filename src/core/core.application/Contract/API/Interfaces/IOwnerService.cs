using core.application.Contract.API.DTO.Party.Owner;

namespace core.application.Contract.API.Interfaces;

public interface IOwnerService
{
    Task<int> CreateOwner(OwnerCreateRequest ownerCreateDTO);
    Task<IEnumerable<OwnerGetResponse>> GetAllOwnersAsync(OwnerGetRequestFilter filter);
    Task<bool> UpdateOwner(OwnerUpdateRequest ownerUpdateDTO);
}
