using core.domain.entity.enums;

namespace core.domain.entity.EnjoyEventModels;

public class EventSessionModel
{
    public long Id { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public EventModel Event { get; set; }
    public int Capacity { get; set; }
    public string Place { get; set; }
    public GenderType GenderType { get; set; }
    public bool IsActive { get; set; }
    public List<EventTicketModel> Tickets { get; set; }
}

