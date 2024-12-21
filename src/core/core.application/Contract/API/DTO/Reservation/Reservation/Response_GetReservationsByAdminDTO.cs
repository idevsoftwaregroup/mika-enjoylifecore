using core.domain.entity.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.application.Contract.API.DTO.Reservation.Reservation
{
    public class Response_GetReservationsByAdminDTO
    {
        public long TotalCount { get; set; }
        public List<Middle_GetReservationsByAdminDTO>? Reservations { get; set; }
    }
    public class Middle_GetReservationsByAdminDTO
    {
        public long ReservationId { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int Count { get; set; }
        public int GuestCount { get; set; }
        public decimal Cost { get; set; }
        public bool CancelledByAdmin { get; set; }
        public bool CancelledByByUser { get; set; }
        public string CancellationDescription { get; set; }
        public DateTime ReservationDate { get; set; }
        public int? LastModifier { get; set; }
        public DateTime? LastModificationDate { get; set; }
        public Middle_GetReservationsByAdmin_JointSessionDTO JointSession { get; set; }
        public Middle_GetReservationsByAdmin_ReservedBy ReservedBy { get; set; }
        public Middle_GetReservationsByAdmin_ReservedForUnit ReservedForUnit { get; set; }

    }
    public class Middle_GetReservationsByAdmin_JointSessionDTO
    {
        public long JointSessionId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime SessionDate { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public DateTime? StartReservationDate { get; set; }
        public TimeSpan? StartReservationTime { get; set; }
        public DateTime? EndReservationDate { get; set; }
        public TimeSpan? EndReservationTime{ get; set; }
        public bool IsPrivate { get; set; }
        public GenderType? PublicSessionGender { get; set; }
        public string? PublicSessionGenderTitle { get; set; }
        public string? PublicSessionGenderDisplayTitle { get; set; }
        public int? Capacity { get; set; }
        public int? RemainCapacity { get; set; }
        public int? GuestCapacity { get; set; }
        public int? RemainGuestCapacity { get; set; }
        public Middle_JointDTO Joint { get; set; }
    }
    public class Middle_GetReservationsByAdmin_ReservedBy
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Email { get; set; }
        public string PhoneNumber { get; set; }
        public string? NationalID { get; set; }
        public string? Address { get; set; }
        public int? Age { get; set; }
        public GenderType? Gender { get; set; }
        public string? GenderTitle { get; set; }
        public string? GenderDisplayTitle { get; set; }

    }
    public class Middle_GetReservationsByAdmin_ReservedForUnit
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Floor { get; set; }
        public int ComplexId { get; set; }
        public int Block { get; set; }
    }

}
