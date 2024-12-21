using core.domain.entity.structureModels;
using core.domain.entity.ticketingModels;

namespace core.application.Contract.API.DTO.Ticket.Message;

public class GetMessageResponseDTO
{
    public long Id { get; set; }
    public int TicketId { get; set; }
    public string Text { get; set; }
    public int AuthorUserId { get; set; }
    public string CreatedAt { get; set; }
    public Attachment Attachment { get; set; }
}


