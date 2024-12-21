using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.domain.DomainModelDTOs.TicketingDTOs
{
    public class Request_CreateTicketMessageDomainDTO
    {
        public long TicketId { get; set; }
        public string? Text { get; set; }
        public string? Url { get; set; }
    }
}
