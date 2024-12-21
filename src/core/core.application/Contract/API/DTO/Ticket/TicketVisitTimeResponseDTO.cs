using core.domain.entity.ticketingModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.application.Contract.API.DTO.Ticket
{
    public class TicketVisitTimeResponseDTO
    {
        public int Id { get; set; }
        public string? Title { get; set; } = "";
        public int? Order { get; set; }
        public string VisitTime { get; set; } 
        //public TicketModel Ticket { get; set; }
    }
}
