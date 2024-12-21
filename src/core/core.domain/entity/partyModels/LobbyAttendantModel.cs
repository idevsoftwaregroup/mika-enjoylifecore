using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.domain.entity.partyModels
{
    public class LobbyAttendantModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LobbyAttendantId { get; set; }
        [MaxLength(50)]
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool Active { get; set; }
        [MaxLength(40)]
        public string FirstName { get; set; }
        [MaxLength(40)]
        public string LastName { get; set; }
        [MaxLength(22)]
        public string PhoneNumber { get; set; }
        public int Creator { get; set; }
        public DateTime CreationDate { get; set; }
        public int? LastModifier { get; set; }
        public DateTime? LastModificationDate { get; set; }
    }
}
