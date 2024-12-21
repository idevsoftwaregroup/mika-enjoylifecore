using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.application.Contract.API.DTO.Reservation.Joint
{
    public class Response_GetJointStatusDTO
    {
        public int JointStatusId { get; set; }
        public string Title { get; set; }
        public string DisplayTitle { get; set; }

    }
}
