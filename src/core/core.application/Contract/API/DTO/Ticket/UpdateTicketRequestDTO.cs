using core.domain.entity.ticketingModels;

namespace core.application.Contract.API.DTO.Ticket;

public class UpdateTicketRequestDTO
{
    //public TicketStatus TicketStatus { get; set; }
    public int? TechnicianId { get; set; }
    public DateTime? VisitTime { get; set; }
    public string? CloseMessage { get; set; }
}
