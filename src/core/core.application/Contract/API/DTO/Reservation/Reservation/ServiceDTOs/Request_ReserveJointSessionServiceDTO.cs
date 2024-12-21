using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.application.Contract.API.DTO.Reservation.Reservation.ServiceDTOs
{
    public class Request_ReserveJointSessionServiceDTO
    {
        public int CustomerId { get; set; }
        public int UnitId { get; set; }
        public long JointSessionId { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public int Count { get; set; }
        public int GuestCount { get; set; }
    }
}
