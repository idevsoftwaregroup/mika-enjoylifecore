using core.domain.entity.EnjoyEventModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.application.Contract.API.DTO.Reservation.Joint.Middles
{
    public class Middle_GetJointMultiMediaDTO
    {
        public long JointMultiMediaId { get; set; }
        public string Url { get; set; }
        public MediaType MediaType { get; set; }
        public string? Alt { get; set; }

    }
}
