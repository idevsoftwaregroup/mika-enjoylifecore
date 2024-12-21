using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.application.Contract.API.DTO.Reservation.Reservation.Filter
{
    public class Filter_GetReservationReportDTO
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? ReservedForUnitId { get; set; }
        public int? ReservedById { get; set; }
    }
}
