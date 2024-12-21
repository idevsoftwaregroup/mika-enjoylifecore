using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mika.Domain.Entities
{
    public class HistoryLogDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long HistoryLogDetailId { get; set; }
        [MaxLength(500)]
        public string Key { get; set; }
        public string Value { get; set; }
        public long HistoryLogId { get; set; }

        public virtual HistoryLog HistoryLog { get; set; }
    }
}
