using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mika.Domain.Entities
{
    public class BiDataEntry
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long BiDataEntryId { get; set; }
        public long BalanceOfThePreviousDay { get; set; }
        public long BlockadeAmount { get; set; }
        public long DepositDuringTheDay { get; set; }
        public long WithdrawalDuringTheDay { get; set; }
        public long BalanceWithTheBank { get; set; }
        public long OnTheWayReceivablePayableDocs  { get; set; }
        public long DefinitiveReceivablePayableDocs { get; set; }
        public long WithdrawalBalance { get; set; }
        public long AccountBalance { get; set; }
        public DateTime EntryDateTime { get; set; }
        public long AccountId { get; set; }
        public long CreatorId { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? LastModificationDate { get; set; }
     


        public virtual Account Account { get; set; }
        public virtual User Creator { get; set; }
    }
}
