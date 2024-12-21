using common.defination.Utility;
using core.application.Contract.API.DTO.Ticket;
using core.application.Contract.infrastructure;
using core.domain.entity.structureModels;
using core.domain.entity.ticketingModels;
using System.Globalization;

namespace core.application.Contract.API.Mapper
{
    public static class TicketMapper
    {
        //public static TicketDetailResponseDTO ConvertTicketModelToTicketResponseDTO(
        //    this TicketModel value, 
        //    bool Seen, DateTime? SeenDate)
        //{
        //    return new TicketDetailResponseDTO()
        //    {
        //        Id = value.Id,
        //        ticketId = value.Id,
        //        CreatedAt = value.CreatedAt.ToString("HH:mm-yy/MM/dd", new CultureInfo("fa-IR")),
        //        ModifyDate = value.ModifyDate?.ToString("HH:mm-yy/MM/dd", new CultureInfo("fa-IR")),
        //        UserId = value.User != null ? value.User.Id : 0,
        //        CreatedBy = value.User.GetUserFullName(),
        //        Title = value.Title,
        //        Description = value.Description,
        //        TicketStatus = value.TicketStatus,
        //        Attachment = value.Attachment,
        //        UnitId = value.Unit?.Id,
        //        UnitName = value.Unit?.Name,
        //        VisitTime = value.VisitTime?.ToString("yyyy/MM/dd", new CultureInfo("fa-IR")),
        //        Urgent = value.Urgent,
        //        Messages = new(),
        //        TrackingCode = value.TrackingCode,
        //        TicketNumber = value.TicketNumber,
        //        Seen = Seen,
        //        SeenDate = SeenDate != null ? Convert.ToDateTime(SeenDate).ToString("yyyy/MM/dd", new CultureInfo("fa-IR")) : null,
        //        SeenTime = SeenDate != null ? Convert.ToDateTime(SeenDate).ToString("HH:mm", new CultureInfo("fa-IR")) : null,
        //    };
        //}
        //public static TicketPreviewResponseDTO ConvertTicketModelToTicketResponseDTOwithDetail(this TicketModel value, DateTime lastResponseDate, bool Seen, DateTime? SeenDate)
        //{
        //    return new TicketPreviewResponseDTO()
        //    {
        //        Id = value.Id,
        //        ticketId = value.Id,
        //        TrackingCode = value.TrackingCode,
        //        CreatedAt = value.CreatedAt.ToString("yyyy/MM/dd", new CultureInfo("fa-IR")),
        //        Urgent = value.Urgent,
        //        TicketStatus = value.TicketStatus,
        //        Title = value.Title,
        //        CreatedBy = value.User.GetUserFullName(),
        //        LastResponseDate = lastResponseDate == default ? null : lastResponseDate.ToString("yyyy/MM/dd", new CultureInfo("fa-IR")),
        //        TechnicianName = value.Technician == null ? null : value.Technician.FirstName + " " + value.Technician.LastName,
        //        VisitTime = value.VisitTime.ConvertToSimpleDateTimePersian(),
        //        Attachment = value.Attachment,
        //        UnitName = value.Unit?.Name,
        //        TicketNumber = value.TicketNumber,
        //        Seen = Seen,
        //        SeenDate = SeenDate != null ? Convert.ToDateTime(SeenDate).ToString("yyyy/MM/dd", new CultureInfo("fa-IR")) : null,
        //        SeenTime = SeenDate != null ? Convert.ToDateTime(SeenDate).ToString("HH:mm", new CultureInfo("fa-IR")) : null,
        //    };
        //}
        //private static string GetUserFullName(this UserModel user)
        //{
        //    if (user == null)
        //    {
        //        return string.Empty;
        //    }
        //    var firstName = !string.IsNullOrEmpty(user.FirstName) && !string.IsNullOrWhiteSpace(user.FirstName) ? user.FirstName.Trim() : string.Empty;
        //    var lastName = !string.IsNullOrEmpty(user.LastName) && !string.IsNullOrWhiteSpace(user.LastName) ? user.LastName.Trim() : string.Empty;
        //    return $"{firstName} {lastName}".Trim();
        //}
    }
}
