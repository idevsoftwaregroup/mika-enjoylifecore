using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.domain.DomainModelDTOs.BillDTOs
{
    public class Middle_BillFlatDTO
    {

        public long BillId { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public string Description { get; set; }
        public int UnitId { get; set; }
        public string UnitName { get; set; }

    }
}
