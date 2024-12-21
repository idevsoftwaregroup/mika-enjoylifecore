using core.domain.entity.financialModels;
using core.domain.entity.structureModels;

namespace core.application.Contract.Infrastructure
{
    public interface IAccountRepository
    {
        AccountModel GetAccount(int complexId, int? accountType);
        Task<List<MenuModel>> GetUserMenus(List<string> roleNames, CancellationToken cancellation = default);
    }
}
