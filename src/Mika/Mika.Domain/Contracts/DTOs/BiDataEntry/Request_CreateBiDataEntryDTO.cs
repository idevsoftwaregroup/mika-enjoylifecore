using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mika.Domain.Contracts.DTOs.BiDataEntry
{
    public class Request_CreateBiDataEntryDTO
    {
        public long BalanceOfThePreviousDay { get; set; }
        public long BlockadeAmount { get; set; }
        public long DepositDuringTheDay { get; set; }
        public long WithdrawalDuringTheDay { get; set; }
        public long OnTheWayReceivablePayableDocs { get; set; }
        public long DefinitiveReceivablePayableDocs { get; set; }
        public DateTime EntryDateTime { get; set; }
        public long AccountId { get; set; }
    }
}
