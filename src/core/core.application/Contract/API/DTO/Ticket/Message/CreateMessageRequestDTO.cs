namespace core.application.Contract.API.DTO.Ticket.Message;

public class CreateMessageRequestDTO
{
    public long TicketId { get; set; }
    public string? Text { get; set; }
    public string? Url { get; set; }

}
