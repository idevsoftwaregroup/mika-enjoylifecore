using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.domain.entity.log
{
    public class Module
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [MaxLength(70)]
        public string Name { get; set; }
        [MaxLength(90)]
        public string DisplayName { get; set; }

        public virtual List<ActionLookup> ActionLookups { get; set; }
    }
}
