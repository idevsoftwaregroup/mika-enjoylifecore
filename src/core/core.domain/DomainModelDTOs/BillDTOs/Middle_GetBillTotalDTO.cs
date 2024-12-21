using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.domain.DomainModelDTOs.BillDTOs
{
    public class Middle_GetBillTotalDTO
    {
        public int UnitId { get; set; }
        public string UnitName { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public decimal OutstandingDebt { get; set; }
    }
}
