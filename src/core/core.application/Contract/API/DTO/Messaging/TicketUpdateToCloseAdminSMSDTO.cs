using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.application.Contract.API.DTO.Messaging
{
    public class TicketUpdateToCloseAdminSMSDTO
    {
        public string CancelationType { get; set; }
        public string UnitName { get; set; }
        public string? TicketNumber { get; set; }
        public string TrackingCode { get; set; }
        public string CreatedDate { get; set; }
        public string Title { get; set; }
        public string? CloseMessage { get; set; }
        public string ClosedDate { get; set; }
    }
}
