using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.application.Contract.API.DTO.Reservation.Reservation.Filter
{
    public class Filter_GetJointSessionByAdminDTO
    {
        public string? Search { get; set; }
        public DateTime? FromSessionDate { get; set; }
        public DateTime? ToSessionDate { get; set; }
        public bool? IsClosure { get; set; }
        public DateTime? FromReservationDate { get; set; }
        public DateTime? ToReservationDate { get; set; }
        public bool? IsPrivate { get; set; }
        public int? JointId { get; set; }
        public int? PageNumber { get; set; } = 1;
        public int? PageSize { get; set; } = 20;
    }
}
