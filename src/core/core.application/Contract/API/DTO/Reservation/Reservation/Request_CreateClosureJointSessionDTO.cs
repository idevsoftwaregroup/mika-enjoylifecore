using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.application.Contract.API.DTO.Reservation.Reservation
{
    public class Request_CreateClosureJointSessionDTO
    {
        public int JointId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime SessionDate { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }

    }
}
