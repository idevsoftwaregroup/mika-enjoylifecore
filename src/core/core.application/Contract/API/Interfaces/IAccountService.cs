using core.application.Contract.API.DTO.Account;
using core.application.Contract.API.DTO.Menu;

namespace core.application.Contract.API.Interfaces;

public interface IAccountService
{
    Task<AccountGetResponseDTO> GetAccount(int complexId, int? accountType);
    Task<List<MenuDTO>> GetUserMenu(List<string> roleNames, CancellationToken cancellationToken = default);
}
