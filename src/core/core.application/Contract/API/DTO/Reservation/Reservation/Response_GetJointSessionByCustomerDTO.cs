using core.domain.entity.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.application.Contract.API.DTO.Reservation.Reservation
{
    public class Response_GetJointSessionByCustomerDTO
    {
        public DateTime RequestDate { get; set; }
        public List<Middle_GetJointSessionsByCustomerDTO>? TodayJointSessions { get; set; }
        public List<Middle_GetJointSessionsByCustomerDTO>? AvailableJointSessions { get; set; }
    }
    public class Middle_GetJointSessionsByCustomerDTO
    {
        public long JointSessionId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime SessionDate { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public bool IsClosure { get; set; }
        public DateTime? StartReservationDate { get; set; }
        public TimeSpan? StartReservationTime { get; set; }
        public DateTime? EndReservationDate { get; set; }
        public TimeSpan? EndReservationTime { get; set; }
        public int? MinimumReservationMinutes { get; set; }
        public int? MaximumReservationMinutes { get; set; }
        public decimal? SessionCost { get; set; }
        public bool IsPrivate { get; set; }
        public GenderType? PublicSessionGender { get; set; }
        public string? PublicSessionGenderTitle { get; set; }
        public string? PublicSessionGenderDisplay { get; set; }
        public bool UnitHasAccessForExtraReservation { get; set; }
        public decimal? UnitExtraReservationCost { get; set; }
        public int? Capacity { get; set; }
        public int? GuestCapacity { get; set; }
        public bool IsAvailable { get; set; }
        public string? ForbiddenText { get; set; }
        public Middle_JointDTO Joint { get; set; }
    }
}
