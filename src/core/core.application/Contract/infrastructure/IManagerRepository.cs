using core.application.Contract.API.DTO.Party.Manager;
using core.domain.entity.partyModels;

namespace core.application.Contract.Infrastructure;

public interface IManagerRepository
{
    Task<int> AddManagerAsync(ManagerCreateRequest managerCreateRequest);
    List<ManagerModel> GetComplexManagers(int complexId);
    ManagerModel GetManager(int managerId);
    Task<int> UpdateManagerAsync(ManagerUpdateRequest managerUpdateRequest);
}
