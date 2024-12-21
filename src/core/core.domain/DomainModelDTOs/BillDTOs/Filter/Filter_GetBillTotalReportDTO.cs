using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.domain.DomainModelDTOs.BillDTOs.Filter
{
    public class Filter_GetBillTotalReportDTO
    {
        public int? UnitId { get; set; }
        public int? Year { get; set; }
        public List<int>? Months { get; set; }
    }
}
