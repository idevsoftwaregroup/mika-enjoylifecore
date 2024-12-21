using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.application.Contract.API.DTO.Reservation.Reservation
{
    public class Response_CancelReservationDTO
    {
        public string ReservedByGenderDisplayTitle { get; set; }
        public string ReservedByFullName { get; set; }
        public string JonitTitle { get; set; }
        public DateTime SessionDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string? CancellationDescription { get; set; }
        public string TargetPhoneNumber { get; set; }
    }
}
