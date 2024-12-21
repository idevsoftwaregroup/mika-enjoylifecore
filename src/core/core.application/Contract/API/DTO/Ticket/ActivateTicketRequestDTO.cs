using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.application.Contract.API.DTO.Ticket
{
    public class ActivateTicketRequestDTO
    {
        public long Id { get; set; }
        public string TrackingCode { get; set; }
        public string? TicketNumber { get; set; }
    }
}
