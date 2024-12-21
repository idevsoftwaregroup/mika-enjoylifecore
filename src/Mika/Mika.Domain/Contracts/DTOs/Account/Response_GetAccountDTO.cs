using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mika.Domain.Contracts.DTOs.Account
{
    public class Response_GetAccountDTO
    {
        public long AccountId { get; set; }
        public string AccountNumber { get; set; }
        public string BankName { get; set; }
        public string AccountType { get; set; }
        public decimal InterestRatePercent { get; set; }
        public string ProjectName { get; set; }
        public bool? BelongsToCentralOffice { get; set; }
        public string Description { get; set; }
        public long CompanyId { get; set; }
        public string CompanyNumber { get; set; }
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? LastModificationDate { get; set; }

    }
}
