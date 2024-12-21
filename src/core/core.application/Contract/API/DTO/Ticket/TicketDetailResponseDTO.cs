using core.domain.entity.ticketingModels;

namespace core.application.Contract.API.DTO.Ticket;

public class TicketDetailResponseDTO
{
    public int Id { get; set; }
    public int ticketId { get; set; }
    public int TrackingCode { get; set; }
    public string? TicketNumber { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public int UserId { get; set; }
    public string? CreatedBy { get; set; }
    public string CreatedAt { get; set; }
    public string? ModifyDate { get; set; }

    public int? UnitId { get; set; }
    public string? UnitName { get; set; }
    public string? LastResponseDate { get; set; }
    public Attachment? Attachment { get; set; }
    public List<MessageResponseDTO> Messages { get; set; }
    public string? TechnicianName { get; set; }
    public string? VisitTime { get; set; }
    public bool Urgent { get; set; }
    //public TicketStatus TicketStatus { get; set; }
    public bool Seen { get; set; }
    public string? SeenDate { get; set; }
    public string? SeenTime {  get; set; }


}
public class MessageResponseDTO
{
    public long Id { get; set; }
    public string UserName { get; set; }
    public bool FromOperator { get; set; }
    public string CreatedAt { get; set;}
    public string Description { get; set;}
    public Attachment? Attachment { get; set; }
}
