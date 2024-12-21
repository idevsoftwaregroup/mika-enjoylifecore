
using core.domain.entity.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.application.Contract.API.DTO.Reservation.Reservation
{
    public class Response_GetJointSessionByAdminDTO
    {
        public long TotalCount { get; set; }
        public List<Middle_GetJointSessionDTO>? JointSessions { get; set; }
    }

    public class Middle_GetJointSessionDTO
    {
        public long JointSessionId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime SessionDate { get; set; }
        public string SessionDateDay { get; set; }
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
        public string? PublicSessionGenderDisplayTitle { get; set; }
        public bool UnitHasAccessForExtraReservation { get; set; }
        public decimal? UnitExtraReservationCost { get; set; }
        public int? Capacity { get; set; }
        public int? GuestCapacity { get; set; }
        public DateTime CreationDate { get; set; }
        public int Creator { get; set; }
        public DateTime? LastModificationDate { get; set; }
        public int? LastModifier { get; set; }
        public Middle_JointDTO Joint { get; set; }
        public List<Middle_AcceptableUnitDTO>? AcceptableUnits { get; set; }
        public bool IsEditable { get; set; }
        public Middle_CreateJointSessions_TimeConcept? CancellationTo { get; set; }
    }
    public class Middle_JointDTO
    {
        public int JointId { get; set; }
        public string Title { get; set; }
        public string? Location { get; set; }
        public List<string>? PhoneNumbers { get; set; }
        public string? Description { get; set; }
        public string? TermsText { get; set; }
        public string? TermsFileUrl { get; set; }
        public string? ThumbnailUrl { get; set; }
        public int? DailyUnitReservationCount { get; set; }
        public int? WeeklyUnitReservationCount { get; set; }
        public int? MonthlyUnitReservationCount { get; set; }
        public int? YearlyUnitReservationCount { get; set; }
        public List<Middle_JointActivityHoursDTO> ActivityHours { get; set; }
    }
    public class Middle_JointActivityHoursDTO
    {
        public TimeSpan PartialStartTime { get; set; }
        public TimeSpan PartialEndTime { get; set; }
    }
    public class Middle_AcceptableUnitDTO
    {
        public int UnitId { get; set; }
        public string UnitName { get; set; }

    }
}
