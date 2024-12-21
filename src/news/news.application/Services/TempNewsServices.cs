using news.application.Contracts.Interfaces;
using news.application.DomainModelDTOs;
using news.application.DomainModelDTOs.FilterDTOs;
using news.application.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace news.application.Services
{
    public class TempNewsServices : ITempNewsServices
    {
        private readonly ITempNewsRepository _tempNewsRepository;
        public TempNewsServices(ITempNewsRepository tempNewsRepository)
        {
            this._tempNewsRepository = tempNewsRepository;
        }



        public async Task<OperationResult<Admin_GetTempNewsArticleDomainDTO>> GetAdminTempNewsArticle(Admin_TempNewsArticle_FilterDTO filter, CancellationToken cancellationToken = default)
        {
            return await this._tempNewsRepository.GetAdminTempNewsArticle(filter, cancellationToken);
        }

        public async Task<OperationResult<Customer_GetTempNewsArticleDomainDTO>> GetCustomerTempNewsArticle(Customer_TempNewsArticle_FilterDTO filter, CancellationToken cancellationToken = default)
        {
            return await _tempNewsRepository.GetCustomerTempNewsArticle(filter, cancellationToken);
        }

        public async Task<OperationResult<object>> PostTempNewsArticle(PostTempNewsArticleDomainDTO model, int adminId, CancellationToken cancellationToken = default)
        {
            return await _tempNewsRepository.PostTempNewsArticle(model, adminId, cancellationToken);
        }
        public async Task<OperationResult<object>> DeleteTempNewsArticle(DeleteTempNewsArticleDomainDTO model, CancellationToken cancellationToken = default)
        {
            return await _tempNewsRepository.DeleteTempNewsArticle(model, cancellationToken);
        }

        public async Task<OperationResult<object>> UpdateTempNewsArticle(UpdateTempNewsArticleDomainDTO model, int adminId, CancellationToken cancellationToken = default)
        {
            return await _tempNewsRepository.UpdateTempNewsArticle(model, adminId, cancellationToken);
        }

        public async Task<OperationResult<Customer_GetSingleTempNewsArticleDomainDTO>> GetSingleTempNewsArticle(long Id, CancellationToken cancellationToken = default)
        {
            return await _tempNewsRepository.GetSingleTempNewsArticle(Id, cancellationToken);
        }

        public async Task<OperationResult<Admin_GetSingleTempNewsArticleDomainDTO>> GetAdminSingleTempNewsArticle(long Id, CancellationToken cancellationToken = default)
        {
            return await _tempNewsRepository.GetAdminSingleTempNewsArticle(Id, cancellationToken);
        }
    }
}
