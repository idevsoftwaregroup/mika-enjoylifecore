namespace core.application.Contract.API.DTO.Ticket;

public class GetTicketListQueryParams
{
    public int? unitId { get; set; }
    public bool? Open { get; set; }
    public bool? Pending { get; set; }
    public bool? Urgent { get; set; }

}
