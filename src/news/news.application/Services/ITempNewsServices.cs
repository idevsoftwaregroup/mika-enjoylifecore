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
    public interface ITempNewsServices
    {
        Task<OperationResult<Admin_GetTempNewsArticleDomainDTO>> GetAdminTempNewsArticle(Admin_TempNewsArticle_FilterDTO filter, CancellationToken cancellationToken = default);
        Task<OperationResult<Admin_GetSingleTempNewsArticleDomainDTO>> GetAdminSingleTempNewsArticle(long Id, CancellationToken cancellationToken = default);
        Task<OperationResult<Customer_GetTempNewsArticleDomainDTO>> GetCustomerTempNewsArticle(Customer_TempNewsArticle_FilterDTO filter, CancellationToken cancellationToken = default);
        Task<OperationResult<Customer_GetSingleTempNewsArticleDomainDTO>> GetSingleTempNewsArticle(long Id, CancellationToken cancellationToken = default);
        Task<OperationResult<object>> PostTempNewsArticle(PostTempNewsArticleDomainDTO model, int adminId, CancellationToken cancellationToken = default);
        Task<OperationResult<object>> UpdateTempNewsArticle(UpdateTempNewsArticleDomainDTO model, int adminId, CancellationToken cancellationToken = default);
        Task<OperationResult<object>> DeleteTempNewsArticle(DeleteTempNewsArticleDomainDTO model, CancellationToken cancellationToken = default);
    }
}
