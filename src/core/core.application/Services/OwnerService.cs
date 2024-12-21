using core.application.Contract.API.DTO.Party.Owner;
using core.application.Contract.API.Interfaces;
using core.application.Contract.API.Mapper;
using core.application.Contract.Infrastructure;
using core.domain.entity.partyModels;

namespace core.application.Services
{
    public class OwnerService : IOwnerService
    {
        public IOwnerRepository _ownerRepository { get; set; }

        public OwnerService(IOwnerRepository ownerRepository)
        {
            _ownerRepository = ownerRepository;
        }

        public async Task<IEnumerable<OwnerGetResponse>> GetAllOwnersAsync(OwnerGetRequestFilter filter)
        {
            List<OwnerModel> listowners = _ownerRepository.GetOwners(filter);
            return listowners.Select(x => x.ConvertOwnerModelToGetResponse()).ToList();
        }

        public async Task<int> CreateOwner(OwnerCreateRequest ownerCreateDTO)
        {
            int response = await _ownerRepository.AddAsync(ownerCreateDTO);
            return response;
        }
        public async Task<bool> UpdateOwner(OwnerUpdateRequest ownerUpdateDTO)
        {

            return await _ownerRepository.UpdateAsync(ownerUpdateDTO) > 0;
        }
    }
}
