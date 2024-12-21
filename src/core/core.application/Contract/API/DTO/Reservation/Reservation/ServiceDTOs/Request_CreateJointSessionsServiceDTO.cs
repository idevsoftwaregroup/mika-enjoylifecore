using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.application.Contract.API.DTO.Reservation.Reservation.ServiceDTOs
{
    public class Request_CreateJointSessionsServiceDTO
    {
        public int JointId { get; set; }
        public string Type { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public List<int> Days { get; set; }
        public List<Middle_CreateJointSessions_TimesService> Times { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }

        //---------------- Restictions ----------------
        public int? PublicSessionGender { get; set; }
        public Middle_CreateJointSessions_TimeConcept? StartReservationFrom { get; set; }
        public Middle_CreateJointSessions_TimeConcept? EndReservationTo { get; set; }
        public decimal? SessionCost { get; set; }
        public bool? HasUnitMoreReservationAccess { get; set; }
        public decimal? UnitMoreReservationAccessCost { get; set; }
        public int? Capacity { get; set; }
        public int? GuestCapacity { get; set; }
        public Middle_CreateJointSessions_TimeConcept? CancellationTo { get; set; }
        public List<int>? AcceptableUnitIDs { get; set; }
    }
    public class Middle_CreateJointSessions_TimesService
    {
        public string StartTime { get; set; }
        public string EndTime { get; set; }
    }
}
