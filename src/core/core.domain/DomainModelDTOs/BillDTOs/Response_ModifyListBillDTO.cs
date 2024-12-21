using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.domain.DomainModelDTOs.BillDTOs
{
    public class Response_ModifyListBillDTO
    {
        public int RowNumber { get; set; }
        public bool Success { get; set; }
        public List<string>? Messages { get; set; }
    }
}
