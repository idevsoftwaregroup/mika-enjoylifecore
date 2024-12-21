using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mika.Domain.Contracts.DTOs.Company.Filter
{
    public class Filter_GetCompanyDTO
    {
        public long? CompanyId { get; set; }
        public string? Search { get; set; }
        public DateTime? CreationDate { get; set; }
    }
}
