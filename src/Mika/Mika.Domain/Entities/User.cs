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
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long UserId { get; set; }
        [MaxLength(100)]
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool Active { get; set; }
        [MaxLength(35)]
        public string Name { get; set; }
        [MaxLength(35)]
        public string LastName { get; set; }
        [MaxLength(22)]
        public string PhoneNumber { get; set; }
        [MaxLength(70)]
        public string Email { get; set; }
        [MaxLength(45)]
        public string Position { get; set; }
        [MaxLength(24)]
        public string PersonnelNumber { get; set; }
        [MaxLength(400)]
        public string Avatar { get; set; }
        public int RoleId { get; set; }
        public DateTime CreationDate { get; set; }
        public long Creator { get; set; }
        public DateTime? LastModificationDate { get; set; }
        public long? LastModifier { get; set; }
        public long? PrimaryParentId { get; set; }
        public string SecodaryParentIDs { get; set; }

        public virtual Role Role { get; set; }
        public virtual List<F_UserAccount> F_UserAccounts { get; set; }
        public virtual List<BiDataEntry> BiDataEntries { get; set; }
        public virtual List<HistoryLog> HistoryLogs { get; set; }
        public virtual List<F_UserSubModule> F_UserSubModules { get; set; }
    }
}
