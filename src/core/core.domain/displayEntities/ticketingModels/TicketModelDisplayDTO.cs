using core.domain.displayEntities.partyModels;
using core.domain.displayEntities.structureModels;
using core.domain.entity.structureModels;
using core.domain.entity.ticketingModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace core.domain.displayEntities.ticketingModels
{
    public class TicketModelDisplayDTO
    {
        public int Id { get; set; }
        public int TrackingCode { get; set; }
        public string? TicketNumber { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; }
        public UserModelDisplayDTO User { get; set; }
        public UnitModelDisplayDTO Unit { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifyDate { get; set; }
      //  public TicketStatus TicketStatus { get; set; }
        public DateTime? VisitTime { get; set; }
        public bool Urgent { get; set; }
        public bool Seen { get; set; }
        public string? SeenDate { get; set; }
        public string? SeenTime { get; set; }
    }
}
