using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.application.Contract.API.DTO.Reservation.Reservation
{
    public class Request_ReserveJointSessionDTO
    {
        public int CustomerId { get; set; }
        public int UnitId { get; set; }
        public long JointSessionId { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int Count { get; set; }
        public int GuestCount { get; set; }
    }
}
