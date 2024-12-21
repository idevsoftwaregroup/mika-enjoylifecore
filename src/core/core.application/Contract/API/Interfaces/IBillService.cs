using core.application.Framework;
using core.domain.DomainModelDTOs.BillDTOs;
using core.domain.DomainModelDTOs.BillDTOs.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.application.Contract.API.Interfaces
{
    public interface IBillService
    {
        Task<OperationResult<Response_GetBillTotalReportDTO>> GetBillTotalReport(Filter_GetBillTotalReportDTO? filter, CancellationToken cancellationToken = default);
        Task<OperationResult<Response_GetBillDetailReportDTO>> GetBillDetailReport(int UnitId, Filter_GetBillDetailReportDTO? filter, CancellationToken cancellationToken = default);
        Task<OperationResult<Response_InitialFilterDataDTO>> GetBillInitialFilterData(CancellationToken cancellationToken = default);
        Task<OperationResult<Response_ModifyListBillDTO>> ModifyBillsByExcelFile(int ModifierId, List<Request_ModifyListBillDTO> model, CancellationToken cancellationToken = default);
    }
}
