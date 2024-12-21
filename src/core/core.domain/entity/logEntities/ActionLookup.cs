using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.domain.entity.log
{
    public class ActionLookup
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public long ActivityLookupId { get; set; }
        public int ModuleId { get; set; }
        [MaxLength(200)]
        public string ActionName { get; set; }
        [MaxLength(2000)]
        public string ActionDescription { get; set; }
        [MaxLength(150)]
        public string Key { get; set; }
        [MaxLength(1500)]
        public string Value { get; set; }
        public DateTime LogDate { get; set; }


        public virtual ActivityLookup ActivityLookup { get; set; }
        public virtual Module Module { get; set; }
    }
}
