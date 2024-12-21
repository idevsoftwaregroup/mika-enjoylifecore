using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.application.Contract.API.DTO.Reservation.Reservation.Filter
{
    public class Filter_GetReservationsByAdminDTO
    {
        public long? JointSessionId { get; set; }
        public DateTime? JointSessionDateFrom { get; set; }
        public DateTime? JointSessionDateTo { get; set; }
        public int? ReservedForUnitId { get; set; }
        public int? ReservedById { get; set; }
        public bool? CancelledByAdmin { get; set; }
        public bool? CancelledByUser { get; set; }
        public int? PageNumber { get; set; } = 1;
        public int? PageSize { get; set; } = 20;
    }
}
