using core.application.Contract.Infrastructure;
using core.domain.entity.financialModels;
using core.domain.entity.structureModels;
using core.infrastructure.Data.persist;
using Microsoft.EntityFrameworkCore;

namespace core.infrastructure.Data.repository;

public class AccountRepository : IAccountRepository
{
    private EnjoyLifeContext _context;

    public AccountRepository(EnjoyLifeContext context)
    {
        _context = context;
    }

    public AccountModel GetAccount(int complexId, int? accountType)
    {
        try
        {
            if (accountType.HasValue)
            {
                return _context.Accounts
                    .Where(x => x.complex.Id == complexId && x.AccountType == accountType.Value).Include(c => c.complex)
                    .FirstOrDefault();
            }
            else
            {
                return _context.Accounts
                    .Where(x => x.complex.Id == complexId).Include(c => c.complex)
                    .FirstOrDefault();
            }
        }
        catch (Exception e)
        {
            return null;
        }
    }

    public async Task<List<MenuModel>> GetUserMenus(List<string> roleNames, CancellationToken cancellation = default)
    {
        try
        {
            if (roleNames != null && roleNames.Any())
            {
                var result = new List<MenuModel>();
                var menus = (from roleName in roleNames.Select(x => x.ToUpper()).Distinct().ToList()
                             join role in (await _context.Roles.ToListAsync(cancellationToken: cancellation)).Select(x => new RoleModel
                             {
                                 Id = x.Id,
                                 Name = x.Name.ToUpper()
                             }).ToList()
                             on roleName equals role.Name
                             join roleMenu in await _context.RoleMenus.ToListAsync(cancellationToken: cancellation)
                             on role.Id equals roleMenu.RoleId
                             join menu in await _context.Menus.ToListAsync(cancellationToken: cancellation)
                             on roleMenu.MenuId equals menu.Id
                             select menu).ToList();
                if (menus == null || !menus.Any())
                {
                    return result;
                }
                result.AddRange(menus);
                result.AddRange((from menu in menus
                                 join subMenu in await _context.Menus.ToListAsync(cancellationToken: cancellation)
                                 on menu.Id equals subMenu.ParentId
                                 select subMenu).ToList());
                return result;
            }
            return new List<MenuModel>();
        }
        catch (Exception)
        {
            return new List<MenuModel>();
        }
    }
}
