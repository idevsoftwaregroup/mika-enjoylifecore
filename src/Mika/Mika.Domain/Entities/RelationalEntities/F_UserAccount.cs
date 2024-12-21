using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mika.Domain.Entities.RelationalEntities
{
    public class F_UserAccount
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long F_UserAccountId { get; set; }
        public long UserId { get; set; }
        public long AccountId { get; set; }

        public virtual User User { get; set; }
        public virtual Account Account { get; set; }
    }
}
