using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.domain.DomainModelDTOs.BillDTOs
{
    public class Response_InitialFilterDataDTO
    {
        public List<Middle_BillUnitDTO> Units { get; set; }
        public List<Middle_BillYearDTO> Years { get; set; }
        public List<Middle_BillMonthDTO> Months { get; set; }
    }
}
