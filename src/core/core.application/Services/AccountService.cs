using core.application.Contract.API.DTO.Account;
using core.application.Contract.API.DTO.Complex;
using core.application.Contract.API.DTO.Menu;
using core.application.Contract.API.Interfaces;
using core.application.Contract.API.Mapper;
using core.application.Contract.Infrastructure;
using core.domain.entity.financialModels;
using core.domain.entity.structureModels;

namespace core.application.Services
{
    public class AccountService : IAccountService
    {
        public IAccountRepository _IaccountRepository { get; set; }


        public AccountService(IAccountRepository accountRepository)
        {
            _IaccountRepository = accountRepository;
        }

        public async Task<AccountGetResponseDTO> GetAccount(int complexId, int? accountType)
        {
            AccountModel acccountModel = _IaccountRepository.GetAccount(complexId, accountType);
            if (acccountModel == null)
                return null;
            var dto = acccountModel.ConvertComplexModelToGetComplexResponseDTO();
            return dto;
        }

        public async Task<List<MenuDTO>> GetUserMenu(List<string> roleNames, CancellationToken cancellationToken = default)
        {
            try
            {
                var menus = await _IaccountRepository.GetUserMenus(roleNames, cancellationToken);
                if (menus != null && menus.Any())
                {
                    var result = menus.Where(x => x.ParentId == null).Select(x => new MenuDTO
                    {
                        FontIcon = string.IsNullOrEmpty(x.FontIcon) || string.IsNullOrWhiteSpace(x.FontIcon) ? null : x.FontIcon,
                        Icon = x.Icon,
                        Id = x.Id,
                        Size = x.SizeInPixel,
                        Title = x.Title,
                        Url = x.Url,
                        ListItems = menus.Where(e => e.ParentId == x.Id).Select(y => new SubMenuDTO
                        {
                            FontIcon = string.IsNullOrEmpty(y.FontIcon) || string.IsNullOrWhiteSpace(y.FontIcon) ? null : y.FontIcon,
                            Icon = y.Icon,
                            Id = y.Id,
                            Size = y.SizeInPixel,
                            Title = y.Title,
                            Url = y.Url,
                        }).ToList()
                    }).ToList();

                    return result.OrderBy(x=>x.Id).ToList();
                }
                return new List<MenuDTO>();
            }
            catch (Exception)
            {
                return new List<MenuDTO>();
            }

        }
    }
}
