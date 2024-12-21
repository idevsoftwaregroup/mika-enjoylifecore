using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.application.Contract.API.DTO.Reservation.Reservation.ServiceDTOs
{
    public class Request_UpdateClosureJointSessionServiceDTO
    {
        public int JointSessionId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
    }
}
