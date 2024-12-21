using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mika.Domain.Entities
{
    public class HistoryLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long HistoryLogId { get; set; }
        [MaxLength(45)]
        public string EntityName { get; set; }
        public long EntityId { get; set; }
        public DateTime LogDate { get; set; }
        public int ModuleId { get; set; }
        public long LoggerId { get; set; }
        [MaxLength(500)]
        public string Key { get; set; }
        public string Value { get; set; }

        public virtual Module Module { get; set; }
        public virtual User Logger { get; set; }
        public virtual List<HistoryLogDetail> HistoryLogDetails { get; set; }
    }
}
