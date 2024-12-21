using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.application.Contract.API.DTO.Reservation.Joint.Middles
{
    public class Middle_CreateJoint_DailyActivityHourDTO
    {
        public DateTime PartialStartTime { get; set; }
        public DateTime PartialEndTime { get; set; }
    }
}
