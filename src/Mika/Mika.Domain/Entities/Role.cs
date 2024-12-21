using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mika.Domain.Entities
{
    public class Role
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RoleId { get; set; }
        [MaxLength(45)]
        public string RoleName { get; set; }
        [MaxLength(50)]
        public string DisplayName { get; set; }


        public virtual List<User> Users { get; set; }
    }
}
