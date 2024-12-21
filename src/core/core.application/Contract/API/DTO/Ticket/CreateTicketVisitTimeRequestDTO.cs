using core.domain.entity.ticketingModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.application.Contract.API.DTO.Ticket
{
    public class CreateTicketVisitTimeRequestDTO
    {
        public string? Title { get; set; }
        public int? Order { get; set; }
        public DateTime VisitTime { get; set; } = DateTime.Now;
        public int TicketId { get; set; }
    }
}
