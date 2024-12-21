using core.domain.displayEntities.ticketingModels;
using core.domain.entity.ticketingModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.application.Contract.API.DTO.Ticket
{
    public class TicketsResponseDTO
    {
        public List<TicketModelDisplayDTO>? Tickets { get; set; }
        public int TotalCount { get; set; }
    }
}
