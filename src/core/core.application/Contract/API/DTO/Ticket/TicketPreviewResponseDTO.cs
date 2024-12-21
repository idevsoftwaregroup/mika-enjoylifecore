using core.domain.entity.ticketingModels;

namespace core.application.Contract.API.DTO.Ticket;

public class TicketPreviewResponseDTO
{
    public int Id { get; set; }
    public int ticketId { get; set; }
    public int TrackingCode { get; set; }
    public string? TicketNumber { get; set; }
    public string Title { get; set; }
    public string? CreatedBy { get; set; }
    public string CreatedAt { get; set; }
    public string? UnitName { get; set; }
    public Attachment? Attachment { get; set; }
    //public TicketStatus TicketStatus { get; set; }
    public string? LastResponseDate { get; set; }
    public string? TechnicianName { get; set; }
    public string? VisitTime { get; set; }
    public bool Urgent {  get; set; }
    public bool Seen { get; set; }
    public string? SeenDate { get; set; }
    public string? SeenTime { get; set; }



}
