using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.application.Contract.API.DTO.Ticket
{
    public class ModifyTicketRequestDTO
    {
        public long TicketId { get; set; }
        public string? Message { get; set; }
    }
}
