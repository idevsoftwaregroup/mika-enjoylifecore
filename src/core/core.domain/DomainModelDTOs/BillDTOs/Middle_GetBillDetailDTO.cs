using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.domain.DomainModelDTOs.BillDTOs
{
    public class Middle_GetBillDetailDTO
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public string MonthTitle { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public string Description { get; set; }

    }
}
