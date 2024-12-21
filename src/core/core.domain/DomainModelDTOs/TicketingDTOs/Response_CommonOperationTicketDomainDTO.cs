using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.domain.DomainModelDTOs.TicketingDTOs
{
    public class Response_CommonOperationTicketDomainDTO
    {
        public long TicketId { get; set; }
        public string? TrackingCode { get; set; }
        public string? TicketNumber { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string UnitName { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedByPhoneNumber { get; set; }
        public string? AttachmentUrl { get; set; }
    }
}
