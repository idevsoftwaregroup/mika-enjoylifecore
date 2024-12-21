using core.domain.entity.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.domain.DomainModelDTOs.TicketingDTOs
{
    public class Response_GetTicketDomainDTO
    {
        public long TotalCount { get; set; }
        public List<TicketDomainDTO>? Tickets { get; set; }
    }
    public class TicketDomainDTO
    {
        public long Id { get; set; }
        public string? TrackingCode { get; set; }
        public string? TicketNumber { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
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

    }
    public class TicketUserAndTechnicianDTO
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Email { get; set; }
        public string PhoneNumber { get; set; }
        public GenderType? GenderType { get; set; }
        public string? Gender { get; set; }
    }
    public class TicketUnitDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Floor { get; set; }
        public int ComplexId { get; set; }
        public int Block { get; set; }

    }
}
