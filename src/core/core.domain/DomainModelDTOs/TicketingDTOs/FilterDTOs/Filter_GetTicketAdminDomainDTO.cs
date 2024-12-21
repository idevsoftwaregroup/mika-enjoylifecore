using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.domain.DomainModelDTOs.TicketingDTOs.FilterDTOs
{
    public class Filter_GetTicketAdminDomainDTO
    {
        public long? TicketId { get; set; }
        public string? TrackingCode { get; set; }
        public string? TicketNumber { get; set; }
        public string? Title { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int? StatusCode { get; set; }
        public int? UserId { get; set; }
        public int? UnitId { get; set; }
        public bool? Seen { get; set; }
        public int? PageNumber { get; set; } = 1;
        public int? PageSize { get; set; } = 20;
    }
}
