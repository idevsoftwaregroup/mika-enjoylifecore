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
    public class Module
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ModuleId { get; set; }
        [MaxLength(45)]
        public string ModuleName { get; set; }
        [MaxLength(50)]
        public string DisplayName { get; set; }

        public virtual List<HistoryLog> HistoryLogs { get; set; }
        public virtual List<SubModule> SubModules { get; set; }
    }
}
