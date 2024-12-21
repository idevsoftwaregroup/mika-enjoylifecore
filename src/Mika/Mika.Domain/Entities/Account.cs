using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mika.Domain.Entities.RelationalEntities;

namespace Mika.Domain.Entities
{
    public class Account
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long AccountId { get; set; }
        [MaxLength(70)]
        public string AccountNumber { get; set; }
        [MaxLength(100)]
        public string BankName { get; set; }
        [MaxLength(100)]
        public string AccountType { get; set; }
        public decimal InterestRatePercent { get; set; }
        [MaxLength(100)]
        public string ProjectName { get; set; }
        public bool? BelongsToCentralOffice { get; set; }
        [MaxLength(2000)]
        public string Description { get; set; }
        public long CompanyId { get; set; }
        public DateTime CreationDate { get; set; }
        public long Creator { get; set; }
        public DateTime? LastModificationDate { get; set; }
        public long? LastModifier { get; set; }

        public virtual Company Company { get; set; }
        public virtual List<F_UserAccount> F_UserAccounts { get; set; }
        public virtual List<BiDataEntry> BiDataEntries { get; set; }

    }
}
