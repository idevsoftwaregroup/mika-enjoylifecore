using core.domain.entity.enums;
using core.domain.entity.structureModels;

namespace core.domain.entity.EnjoyEventModels;

public class EventModel
{
    public const int EmtpyUnitOwnerMaxReservation = 5;
    
    public int Id { get; set; }
    public int ComplexId { get; set; }
    public string Name { get; set; }    
    public DateTime ReservationStartDate { get; set; }
    public DateTime PublishDate { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? Period { get; set; }
    public TimeSpan LockTimeout { get; set; }    
    public GenderType? GenderType { get; set; }
    public string WebsiteUrl { get; set; }
    public string SupportPhoneNumber { get; set; }
    public List<EventContentModel> EventContent { get; set; }
    public List<EventSessionModel> EventSession { get; set; }
    public List<EventPartyModel> EventParty { get; set; }
    public int OwnersMaxReservations { get; set; }
    public bool MandatoryConsecutiveReservation { get; set; }
    public bool repeatSessionReservation { get; set; }
    public string? Place { get; set; }
    public string SessionDescription { get; set; }
    public bool IsPinned { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime ModifyDate { get; set; }
}

public enum EventState
{
    COMMINGSOON,
    ONGOING,
    ARCHIVED
}


