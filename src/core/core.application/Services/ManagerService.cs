using core.application.Contract.API.DTO.Party.Manager;
using core.application.Contract.API.Mapper;
using core.application.Contract.Infrastructure;

namespace core.application.Services
{
    public class ManagerService : IManagerService
    {
        private readonly IManagerRepository _managerRepository;


        public ManagerService(IManagerRepository managerRepository)
        {
            _managerRepository = managerRepository;
        }

        public IEnumerable<ManagerGetResponse> GetAllManagersAsync(int complexId)
        {
            return _managerRepository
                .GetComplexManagers(complexId)
                .Select(x => x.ConvertManagerModelTOManagerGetResponse())
                .ToList();
        }

        public ManagerGetResponse GetManagerById(int mangerId)
        {
            return _managerRepository
                .GetManager(mangerId)
                .ConvertManagerModelTOManagerGetResponse();
        }

        public async Task<int> CreateManager(ManagerCreateRequest managerCreateRequest)
        {
            return await _managerRepository.AddManagerAsync(managerCreateRequest);
        }

        public async Task<bool> UpdateManager(ManagerUpdateRequest managerUpdateRequest)
        {
            return await _managerRepository.UpdateManagerAsync(managerUpdateRequest) > 0;
        }
    }
}
