using core.domain.entity.structureModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.domain.entity.WebSocketModels
{
    public class UserConnectionModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public int UserId { get; set; }
        [MaxLength(150)]
        public string? Connection { get; set; }
        public DateTime? ConnectionDate { get; set; }

        public virtual UserModel User { get; set; }
    }
}
