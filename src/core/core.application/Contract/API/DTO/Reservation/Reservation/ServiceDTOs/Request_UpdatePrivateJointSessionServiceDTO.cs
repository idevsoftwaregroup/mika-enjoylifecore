using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.application.Contract.API.DTO.Reservation.Reservation.ServiceDTOs
{
    public class Request_UpdatePrivateJointSessionServiceDTO
    {
        public long JointSessionId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public DateTime? StartReservationDate { get; set; }
        public DateTime? EndReservationDate { get; set; }
        public decimal? SessionCost { get; set; }
        public bool? UnitHasAccessForExtraReservation { get; set; }
        public decimal? UnitExtraReservationCost { get; set; }
        public int? Capacity { get; set; }
        public int? GuestCapacity { get; set; }
        public Middle_CreateJointSessions_TimeConcept? CancellationTo { get; set; }
        public List<int>? AcceptableUnitIDs { get; set; }
    }
}
