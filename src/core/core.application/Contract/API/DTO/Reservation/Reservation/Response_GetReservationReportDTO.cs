using core.domain.entity.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.application.Contract.API.DTO.Reservation.Reservation
{
    public class Response_GetReservationReportDTO
    {
        public string JointTitle { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? UnitName { get; set; }
        public string? UserFullName { get; set; }
        public List<Middle_GetReservationReport_DataDTO> Data { get; set; }
    }
    public class Middle_GetReservationReport_DataDTO
    {
        public long ReservationId { get; set; }
        public bool IsPrivate { get; set; }
        public int? PublicGenderType { get; set; }
        public string? PublicGenderTypeTitle { get; set; }
        public string? PublicGenderTypeDisplayTitle { get; set; }
        public string JointTitle { get; set; }
        public DateTime SessionDate { get; set; }
        public string SessionDateDay { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int Count { get; set; }
        public int GuestCount { get; set; }
        public decimal TotalCost { get; set; }
        public int ReservedById { get; set; }
        public string ReservedByFullName { get; set; }
        public int ReservedForUnitId { get; set; }
        public string ReservedForUnitName { get; set; }
        public DateTime ReservationDate { get; set; }
        public bool IsActive { get; set; }
        public string StateTitle { get; set; }
        public string StateDisplayTitle { get; set; }
        public string? CancellationDescription { get; set; }

    }
}
