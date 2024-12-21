using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mika.Domain.Contracts.DTOs.Account.Filter
{
    public class Filter_GetAccountDTO
    {
        public long? AccountId { get; set; }
        public string? Search { get; set; }
        public bool? BelongsToCentralOffice { get; set; }
        public long? CompanyId { get; set; }
        public DateTime? CreationDate { get; set; }
    }
}
