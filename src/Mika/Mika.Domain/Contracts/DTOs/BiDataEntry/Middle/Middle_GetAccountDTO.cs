using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mika.Domain.Contracts.DTOs.BiDataEntry.Middle
{
    public class Middle_GetAccountDTO
    {
        public long AccountId { get; set; }
        public string AccountNumber { get; set; }
        public string BankName { get; set; }
        public string? AccountType { get; set; }
        public string ProjectName { get; set; }
    }
}
