using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.domain.DomainModelDTOs.TicketingDTOs
{
    public class Request_ModifyTicketDomainDTO
    {
        public long TicketId { get; set; }
        public string? Message { get; set; }
    }
}
