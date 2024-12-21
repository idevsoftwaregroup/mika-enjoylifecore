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
    public class SubModule
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SubModuleId { get; set; }
        [MaxLength(45)]
        public string SubModuleName { get; set; }
        [MaxLength(50)]
        public string SubDisplayName { get; set; }
        public int ModuleId { get; set; }

        public virtual Module Module { get; set; }
        public virtual List<F_UserSubModule> F_UserSubModules { get; set; }
    }
}
