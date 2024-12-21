using core.domain.entity.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.application.Contract.API.DTO.Reservation.Reservation
{
    public class Request_CreatePublicJointSessionDTO
    {
        public int JointId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime SessionDate { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public DateTime? StartReservationDate { get; set; }
        public DateTime? EndReservationDate { get; set; }
        public int? SessionGender { get; set; }
        public int? MinimumReservationMinutes { get; set; }
        public int? MaximumReservationMinutes { get; set; }
        public decimal? SessionCost { get; set; }
        public bool UnitHasAccessForExtraReservation { get; set; }
        public decimal? UnitExtraReservationCost { get; set; }
        public int? Capacity { get; set; }
        public int? GuestCapacity { get; set; }
        public List<int>? AcceptableUnitIDs { get; set; }
    }
}
