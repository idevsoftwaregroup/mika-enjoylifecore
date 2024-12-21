using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.domain.DomainModelDTOs.BillDTOs
{
    public class Response_GetBillDetailReportDTO
    {
        public int UnitId { get; set; }
        public string UnitName { get; set; }
        public List<Middle_GetBillDetailDTO> Bills { get; set; }
        public decimal SumDebit { get; set; }
        public decimal SumCredit { get; set; }
    }
}
