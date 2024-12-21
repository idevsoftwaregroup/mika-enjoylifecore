using core.domain.entity.structureModels;
using core.domain.entity.ticketingModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.domain.entity.TicketModels
{
    public class TicketMessageModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [MaxLength(4000)]
        public string? Text { get; set; }
        public long? AttachmentId { get; set; }
        public int AuthorId { get; set; }
        public long TicketId { get; set; }
        public DateTime CreatedAt { get; set; }


        public Attachment? Attachment { get; set; }
        public virtual UserModel Author { get; set; }
        public virtual TicketModel Ticket { get; set; }
    }
}
