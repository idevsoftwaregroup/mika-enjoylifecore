using core.application.Contract.API.DTO.Party.Manager;

namespace core.application.Contract.Infrastructure;

public interface IManagerService
{
    IEnumerable<ManagerGetResponse> GetAllManagersAsync(int complexId);
    ManagerGetResponse GetManagerById(int mangerId);
    Task<int> CreateManager(ManagerCreateRequest managerCreateRequest);
    Task<bool> UpdateManager(ManagerUpdateRequest managerUpdateRequest);
}
