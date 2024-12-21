using core.application.Contract.API.Interfaces;
using core.application.Contract.infrastructure;
using core.application.Framework;
using core.domain.DomainModelDTOs.BillDTOs;
using core.domain.DomainModelDTOs.BillDTOs.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.application.Services
{
    public class BillService : IBillService
    {
        private readonly IBillRepository _billRepository;
        public BillService(IBillRepository billRepository)
        {
            this._billRepository = billRepository;
        }

        public async Task<OperationResult<Response_GetBillDetailReportDTO>> GetBillDetailReport(int UnitId, Filter_GetBillDetailReportDTO? filter, CancellationToken cancellationToken = default)
        {
            return await _billRepository.GetBillDetailReport(UnitId, filter, cancellationToken);
        }

        public async Task<OperationResult<Response_InitialFilterDataDTO>> GetBillInitialFilterData(CancellationToken cancellationToken = default)
        {
            return await _billRepository.GetBillInitialFilterData(cancellationToken);
        }

        public async Task<OperationResult<Response_GetBillTotalReportDTO>> GetBillTotalReport(Filter_GetBillTotalReportDTO? filter, CancellationToken cancellationToken = default)
        {
            return await _billRepository.GetBillTotalReport(filter, cancellationToken);
        }

        public async Task<OperationResult<Response_ModifyListBillDTO>> ModifyBillsByExcelFile(int ModifierId, List<Request_ModifyListBillDTO> model, CancellationToken cancellationToken = default)
        {
            return await this._billRepository.ModifyBillsByExcelFile(ModifierId, model, cancellationToken);
        }
    }
}
