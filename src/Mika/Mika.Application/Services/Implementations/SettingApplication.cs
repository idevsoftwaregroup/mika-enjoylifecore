using Mika.Application.Services.Interfaces;
using Mika.Domain.Contracts.DTOs.Account;
using Mika.Domain.Contracts.DTOs.Account.Filter;
using Mika.Domain.Contracts.DTOs.Company;
using Mika.Domain.Contracts.DTOs.Company.Filter;
using Mika.Domain.Contracts.DTOs.Modules;
using Mika.Domain.Contracts.DTOs.Role;
using Mika.Framework.Models;
using Mika.Framework.Models.Pagination;
using Mika.Infastructure.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mika.Application.Services.Implementations
{
    public class SettingApplication : ISettingApplication
    {
        private readonly ISettingRepository _settingRepository;
        public SettingApplication(ISettingRepository settingRepository)
        {
            this._settingRepository = settingRepository;
        }

        public async Task<OperationResult<object>> CreateAccount(long UserId, Request_CreateAccountDTO model, CancellationToken cancellationToken = default)
        {
            return await _settingRepository.CreateAccount(UserId, model, cancellationToken);
        }

        public async Task<OperationResult<object>> CreateCompany(long UserId, Request_CreateCompanyDTO model, CancellationToken cancellationToken = default)
        {
            return await _settingRepository.CreateCompany(UserId, model, cancellationToken);
        }

        public async Task<OperationResult<object>> DeleteAccount(long UserId, Request_AccountIdDTO model, CancellationToken cancellationToken = default)
        {
            return await _settingRepository.DeleteAccount(UserId, model, cancellationToken);
        }

        public async Task<OperationResult<object>> DeleteCompany(long UserId, Request_DeleteCompanyDTO model, CancellationToken cancellationToken = default)
        {
            return await _settingRepository.DeleteCompany(UserId, model, cancellationToken);
        }


        public async Task<OperationResult<Response_GetAccountDTO>> GetAccount(long UserId, long AccountId, CancellationToken cancellationToken = default)
        {
            return await _settingRepository.GetAccount(UserId, AccountId, cancellationToken);
        }

        public async Task<OperationResult<PagedList<Response_GetAccountDTO>>> GetAccounts(long UserId, Filter_GetAccountDTO? filter, PageModel? page, CancellationToken cancellationToken = default)
        {
            return await _settingRepository.GetAccounts(UserId, filter, page, cancellationToken);
        }

        public async Task<OperationResult<PagedList<Respone_GetCompanyDTO>>> GetCompanies(long UserId, Filter_GetCompanyDTO? filter, PageModel? page, CancellationToken cancellationToken = default)
        {
            return await _settingRepository.GetCompanies(UserId, filter, page, cancellationToken);
        }

        public async Task<OperationResult<Respone_GetCompanyDTO>> GetCompany(long UserId, long CompanyId, CancellationToken cancellationToken = default)
        {
            return await _settingRepository.GetCompany(UserId, CompanyId, cancellationToken);
        }

        public async Task<OperationResult<Response_GetModuleDTO>> GetModules(long UserId, CancellationToken cancellationToken = default)
        {
            return await this._settingRepository.GetModules(UserId, cancellationToken);
        }

        public async Task<OperationResult<Response_GetRoleDTO>> GetRoles(long UserId, CancellationToken cancellationToken = default)
        {
            return await _settingRepository.GetRoles(UserId, cancellationToken);
        }

        public async Task<OperationResult<object>> UpdateAccount(long UserId, Request_UpdateAccountDTO model, CancellationToken cancellationToken = default)
        {
            return await _settingRepository.UpdateAccount(UserId, model, cancellationToken);
        }

        public async Task<OperationResult<object>> UpdateCompany(long UserId, Request_UpdateCompanyDTO model, CancellationToken cancellationToken = default)
        {
            return await _settingRepository.UpdateCompany(UserId, model, cancellationToken);
        }

        public async Task<OperationResult<object>> UpdateModule(long UserId, Request_UpdateModuleDTO model, CancellationToken cancellationToken = default)
        {
            return await _settingRepository.UpdateModule(UserId, model, cancellationToken);
        }
    }
}
