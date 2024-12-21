using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.application.Contract.API.DTO.Messaging
{
    public class TicketUpdateNumberSMSDTO
    {
        public string Title { get; set; }
        public string TicketNumber { get; set; }
        public string TicketId { get; set; }
    }
}
