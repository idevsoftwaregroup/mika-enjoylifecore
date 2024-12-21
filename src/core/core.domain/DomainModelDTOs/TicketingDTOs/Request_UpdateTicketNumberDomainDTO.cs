using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.domain.DomainModelDTOs.TicketingDTOs
{
    public class Request_UpdateTicketNumberDomainDTO
    {
        public long Id { get; set; }
        public string TicketNumber { get; set; }
    }
}
