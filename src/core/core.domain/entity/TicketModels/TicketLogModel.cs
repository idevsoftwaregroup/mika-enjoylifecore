using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Threading.Tasks;

namespace core.domain.entity.TicketModels
{
    public class TicketLogModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public int StatusId { get; set; }
        public DateTime LogDate { get; set; }
        [MaxLength(1500)]
        public string? Message { get; set; }
        public bool ModifiedByAdmin { get; set; }
        public long TicketId { get; set; }

        public virtual TicketStatusModel Status { get; set; }
        public virtual TicketModel Ticket { get; set; }
    }
}
