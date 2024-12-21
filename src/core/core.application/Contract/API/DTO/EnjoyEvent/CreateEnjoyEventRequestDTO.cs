using core.domain.entity;
using core.domain.entity.EnjoyEventModels;
using core.domain.entity.enums;
using System.ComponentModel.DataAnnotations;

namespace core.application.Contract.API.DTO.EnjoyEvent;

public class CreateEnjoyEventRequestDTO
{
    public int ComplexId { get; set; }
    public string Name { get; set; }
    public DateTime ReservationStartDate { get; set; }
    public DateTime PublishDate { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? Period { get; set; }
    public TimeSpan LockTimeout { get; set; } = TimeSpan.FromSeconds(5);
    public GenderType? GenderType { get; set; }
    public string WebsiteUrl { get; set; }
    [Phone]
    public string SupportPhoneNumber { get; set; }
    //public List<EventContentModel> EventContent { get; set; }
    public int OwnersMaxReservations { get; set; }
    public bool MandatoryConsecutiveReservation { get; set; }
    public bool repeatSessionReservation { get; set; }
    public string? Place { get; set; }
    public string SessionDescription { get; set; }
    public bool IsPinned { get; set; }
}


