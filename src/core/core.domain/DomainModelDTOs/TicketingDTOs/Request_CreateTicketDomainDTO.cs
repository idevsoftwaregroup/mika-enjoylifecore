using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.domain.DomainModelDTOs.TicketingDTOs
{
    public class Request_CreateTicketDomainDTO
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public bool? Urgent { get; set; }
        public int UnitId { get; set; }
        public string? Url { get; set; }
    }
}
