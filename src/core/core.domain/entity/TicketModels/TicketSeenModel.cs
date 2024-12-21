using core.domain.entity.structureModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.domain.entity.TicketModels
{
    public class TicketSeenModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public int SeenBy { get; set; }
        public DateTime SeenDate { get; set; }
        public long TicketId { get; set; }

        public virtual UserModel Technician { get; set; }
        public virtual TicketModel Ticket { get; set; }
    }
}
