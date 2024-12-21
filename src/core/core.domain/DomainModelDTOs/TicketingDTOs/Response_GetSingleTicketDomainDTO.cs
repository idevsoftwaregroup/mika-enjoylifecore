using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.domain.DomainModelDTOs.TicketingDTOs
{
    public class Response_GetSingleTicketDomainDTO
    {
        public long Id { get; set; }
        public string? TrackingCode { get; set; }
        public string? TicketNumber { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public string FullCreatedAddTitle { get; set; }
        public DateTime? ModifyDate { get; set; }
        public bool Urgent { get; set; }
        public int StatusCode { get; set; }
        public string StatusTitle { get; set; }
        public string StatusDisplayTitle { get; set; }
        public bool Seen { get; set; }
        public DateTime? SeenDateTime { get; set; }
        public string? SeenDateDisplay { get; set; }
        public string? SeenTimeDisplay { get; set; }
        public TicketUserAndTechnicianDTO User { get; set; }
        public TicketUnitDTO Unit { get; set; }
        public TicketUserAndTechnicianDTO? Technician { get; set; }
        public List<TicketMessageDomainDTO>? Messages { get; set; }
        public DateTime? LastResponseDate { get; set; }
        public string? LastResponseDateDisplay { get; set; }
        public string? LastResponseTimeDisplay { get; set; }

    }
    public class TicketMessageDomainDTO
    {
        public long Id { get; set; }
        public string? Text { get; set; }
        public string? AttachmentUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedAtDateDisplay { get; set; }
        public string CreatedAtTimeDisplay { get; set; }
        public TicketUserAndTechnicianDTO Author { get; set; }
    }

}
