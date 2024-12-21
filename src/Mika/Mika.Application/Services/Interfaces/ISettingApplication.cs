using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mika.Domain.Contracts.DTOs.Company.Filter;
using Mika.Domain.Contracts.DTOs.Company;
using Mika.Domain.Contracts.DTOs.Modules;
using Mika.Framework.Models;
using Mika.Framework.Models.Pagination;
using Mika.Domain.Contracts.DTOs.Account.Filter;
using Mika.Domain.Contracts.DTOs.Account;
using Mika.Domain.Contracts.DTOs.Role;
namespace Mika.Application.Services.Interfaces
{
    public interface ISettingApplication
    {
        Task<OperationResult<Response_GetRoleDTO>> GetRoles(long UserId, CancellationToken cancellationToken = default);
        Task<OperationResult<Response_GetModuleDTO>> GetModules(long UserId, CancellationToken cancellationToken = default);
        Task<OperationResult<object>> UpdateModule(long UserId, Request_UpdateModuleDTO model, CancellationToken cancellationToken = default);
        Task<OperationResult<PagedList<Respone_GetCompanyDTO>>> GetCompanies(long UserId, Filter_GetCompanyDTO? filter, PageModel? page, CancellationToken cancellationToken = default);
        Task<OperationResult<Respone_GetCompanyDTO>> GetCompany(long UserId, long CompanyId, CancellationToken cancellationToken = default);
        Task<OperationResult<object>> CreateCompany(long UserId, Request_CreateCompanyDTO model, CancellationToken cancellationToken = default);
        Task<OperationResult<object>> UpdateCompany(long UserId, Request_UpdateCompanyDTO model, CancellationToken cancellationToken = default);
        Task<OperationResult<object>> DeleteCompany(long UserId, Request_DeleteCompanyDTO model, CancellationToken cancellationToken = default);
        Task<OperationResult<PagedList<Response_GetAccountDTO>>> GetAccounts(long UserId, Filter_GetAccountDTO? filter, PageModel? page, CancellationToken cancellationToken = default);
        Task<OperationResult<Response_GetAccountDTO>> GetAccount(long UserId, long AccountId, CancellationToken cancellationToken = default);
        Task<OperationResult<object>> CreateAccount(long UserId, Request_CreateAccountDTO model, CancellationToken cancellationToken = default);
        Task<OperationResult<object>> UpdateAccount(long UserId, Request_UpdateAccountDTO model, CancellationToken cancellationToken = default);
        Task<OperationResult<object>> DeleteAccount(long UserId, Request_AccountIdDTO model, CancellationToken cancellationToken = default);
    }
}
