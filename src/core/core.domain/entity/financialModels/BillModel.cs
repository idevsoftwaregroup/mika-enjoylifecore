using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using core.domain.entity.structureModels;

namespace core.domain.entity.financialModels
{
    public class BillModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long BillId { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        [MaxLength(2500)]
        public string Description { get; set; }
        public int UnitId { get; set; }
        public DateTime ModificationDate { get; set; }
        public int Modifier { get; set; }

        public virtual UnitModel Unit { get; set; }
    }
}
