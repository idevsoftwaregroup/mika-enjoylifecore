using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mika.Domain.Contracts.DTOs.Account
{
    public class Request_CreateAccountDTO
    {
        public string AccountNumber { get; set; }
        public string BankName { get; set; }
        public string? AccountType { get; set; }
        public decimal InterestRatePercent { get; set; }
        public string? ProjectName { get; set; }
        public bool? BelongsToCentralOffice { get; set; }
        public string? Description { get; set; }
        public long CompanyId { get; set; }

    }
}
