using core.application.Contract.API.DTO.Reservation.Joint.Middles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.application.Contract.API.DTO.Reservation.Joint
{
    public class Response_GetJointDTO
    {
        public int JointId { get; set; }
        public string Title { get; set; }
        public string? Location { get; set; }
        public List<string>? PhoneNumbers { get; set; }
        public string? Description { get; set; }
        public string? TermsText { get; set; }
        public string? TermsFileUrl { get; set; }
        public string? ThumbnailUrl { get; set; }
        public int ComplexId { get; set; }
        public bool IsActive { get; set; }
        public int JointStatusId { get; set; }
        public string JointStatusTitle { get; set; }
        public string JointStatusDisplayTitle { get; set; }
        public int? DailyUnitReservationCount { get; set; }
        public int? WeeklyUnitReservationCount { get; set; }
        public int? MonthlyUnitReservationCount { get; set; }
        public int? YearlyUnitReservationCount { get; set; }
        public List<Middle_GetJointDailyActivityHourDTO> DailyActivityHours { get; set; }
        public List<Middle_GetJointMultiMediaDTO>? MultiMedias { get; set; }
    }

}
