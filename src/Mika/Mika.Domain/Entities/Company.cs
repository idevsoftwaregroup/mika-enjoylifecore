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
    public class Company
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long CompanyId { get; set; }
        [MaxLength(30)]
        public string CompanyNumber { get; set; }
        [MaxLength(20)]
        public string CompanyCode { get; set; }
        [MaxLength(150)]
        public string Name { get; set; }
        public DateTime CreationDate { get; set; }
        public long Creator { get; set; }
        public DateTime? LastModificationDate { get; set; }
        public long? LastModifier { get; set; }


        public virtual List<Account> Accounts { get; set; }

    }
}
