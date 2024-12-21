using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.application.Contract.API.DTO.Reservation.Reservation.Filter
{
    public class Filter_GetReservationsByCustomerDTO
    {
        public int? JointId { get; set; }
        public DateTime? JointSessionDate { get; set; }
        public DateTime? FromReservationDate { get; set; }
        public DateTime? ToReservationDate { get; set; }
        public bool? CancelledByAdmin { get; set; }
        public bool? CancelledByUser { get; set; }

    }
}
