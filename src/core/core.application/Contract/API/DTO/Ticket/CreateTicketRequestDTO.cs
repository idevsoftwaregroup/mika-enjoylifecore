namespace core.application.Contract.API.DTO.Ticket;

public class CreateTicketRequestDTO
{
    public string Title { get; set; }
    public string Description { get; set; }
    public bool? Urgent { get; set; }
    public int UnitId { get; set; }
    public string? Url { get; set; }
}
