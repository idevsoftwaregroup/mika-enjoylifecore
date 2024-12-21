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
    public class TicketModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [MaxLength(35)]
        public string? TrackingCode { get; set; }
        [MaxLength(35)]
        public string? TicketNumber { get; set; }
        [MaxLength(1000)]
        [Required]
        public string Title { get; set; }
        [MaxLength(3500)]
        [Required]
        public string Description { get; set; }
        public int StatusId { get; set; }
        public int UserId { get; set; }
        public int UnitId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifyDate { get; set; }
        public int? TechnicianId { get; set; }
        public bool Urgent { get; set; }


        public virtual TicketStatusModel Status { get; set; }
        public virtual UserModel User { get; set; }
        public virtual UnitModel Unit { get; set; }
        public virtual UserModel? Technician { get; set; }
        public virtual List<TicketSeenModel> Seens { get; set; }
        public virtual List<TicketMessageModel> Messages { get; set; }
        public virtual List<TicketLogModel> Logs { get; set; }
    }
}
