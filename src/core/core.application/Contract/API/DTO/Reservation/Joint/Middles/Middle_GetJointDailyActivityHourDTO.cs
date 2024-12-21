using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.application.Contract.API.DTO.Reservation.Joint.Middles
{
    public class Middle_GetJointDailyActivityHourDTO
    {
        public int JointDailyActivityHourId { get; set; }
        public TimeSpan PartialStartTime { get; set; }
        public TimeSpan PartialEndTime { get; set; }
    }
}
