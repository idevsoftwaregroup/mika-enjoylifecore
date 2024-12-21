using Mika.Domain.Contracts.DTOs.BiDataEntry.Middle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mika.Domain.Contracts.DTOs.BiDataEntry
{
    public class Response_GetBiDataEntryDTO
    {
        public long BiDataEntryId { get; set; }
        public DateTime EntryDateTime { get; set; }
        public long BalanceOfThePreviousDay { get; set; }
        public long BlockadeAmount { get; set; }
        public long DepositDuringTheDay { get; set; }
        public long WithdrawalDuringTheDay { get; set; }
        public long BalanceWithTheBank { get; set; }
        public long OnTheWayReceivablePayableDocs { get; set; }
        public long DefinitiveReceivablePayableDocs { get; set; }
        public long WithdrawalBalance { get; set; }
        public long AccountBalance { get; set; }
        public Middle_GetCreatorDTO Creator { get; set; }
        public Middle_GetAccountDTO Account { get; set; }
        public Middle_GetCompanyDTO Company { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? LastModificationDate { get; set; }
    }
  
}
