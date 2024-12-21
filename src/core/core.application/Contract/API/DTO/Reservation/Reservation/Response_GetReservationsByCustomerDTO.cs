using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.application.Contract.API.DTO.Reservation.Reservation
{
    public class Response_GetReservationsByCustomerDTO
    {
        public long ReservationId { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int Count { get; set; }
        public int GuestCount { get; set; }
        public decimal Cost { get; set; }
        public bool CancelledByAdmin { get; set; }
        public bool CancelledByByUser { get; set; }
        public string CancellationDescription { get; set; }
        public bool Cancellable { get; set; }
        public DateTime ReservationDate { get; set; }
        public Middle_GetReservationsByAdmin_JointSessionDTO JointSession { get; set; }
        public Middle_GetReservationsByAdmin_ReservedForUnit ReservedForUnit { get; set; }
    }
}
