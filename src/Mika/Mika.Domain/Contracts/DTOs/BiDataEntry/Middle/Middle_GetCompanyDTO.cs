using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mika.Domain.Contracts.DTOs.BiDataEntry.Middle
{
    public class Middle_GetCompanyDTO
    {
        public long CompanyId { get; set; }
        public string CompanyNumber { get; set; }
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
    }
}
