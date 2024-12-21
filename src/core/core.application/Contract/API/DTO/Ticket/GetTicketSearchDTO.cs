using core.domain.entity.ticketingModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.application.Contract.API.DTO.Ticket
{
    public class GetTicketSearchDTO
    {
        //public TicketStatus? Status {  get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    public class GetAdminTicketSearchDTO
    {
        public int? TrackCode { get; set; }
        public string? Title { get; set; }
        public int? UserId { get; set; }
        public int? UnitId { get; set; }
        //public TicketStatus? Status { get; set; }
        public DateTime? CreateDate { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int? TotalCount { get; set; }
    }
}
