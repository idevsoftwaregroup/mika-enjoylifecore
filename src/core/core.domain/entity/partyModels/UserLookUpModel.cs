using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using core.domain.entity.structureModels;

namespace core.domain.entity.partyModels
{
    public class UserLookUpModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public int UserId { get; set; }
        [MaxLength(700)]
        public string Key { get; set; }
        [MaxLength(2000)]
        public string? StringValue { get; set; }
        public int? IntegerValue { get; set; }
        public long? LongValue { get; set; }
        public decimal? DecimalValue { get; set; }
        public bool? BooleanValue { get; set; }

        public virtual UserModel User { get; set; }
    }
}
