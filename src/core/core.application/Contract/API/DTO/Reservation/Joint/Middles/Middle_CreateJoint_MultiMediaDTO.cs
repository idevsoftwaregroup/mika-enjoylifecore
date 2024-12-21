using core.domain.entity.EnjoyEventModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.application.Contract.API.DTO.Reservation.Joint.Middles
{
    public class Middle_CreateJoint_MultiMediaDTO
    {
        public string Url { get; set; }
        public MediaType? MediaType { get; set; }
        public string? Alt { get; set; }
        public bool? IsThumbnail { get; set; }
    }
}
