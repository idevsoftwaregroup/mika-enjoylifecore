using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.application.Contract.API.DTO.Reservation.Reservation
{
    public class Request_CreateSingleJointSessionDTO
    {
        public int JointId { get; set; }
        public string Type { get; set; }
        public DateTime SessionDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }

        //---------------- Restictions ----------------

        public int? PublicSessionGender { get; set; }
        public DateTime? StartReservationFrom { get; set; }
        public DateTime? EndReservationTo { get; set; }
        public decimal? SessionCost { get; set; }
        public bool? HasUnitMoreReservationAccess { get; set; }
        public decimal? UnitMoreReservationAccessCost { get; set; }
        public int? Capacity { get; set; }
        public int? GuestCapacity { get; set; }
        public Middle_CreateJointSessions_TimeConcept? CancellationTo { get; set; }
        public List<int>? AcceptableUnitIDs { get; set; }
    }
}
