namespace core.application.Contract.API.DTO.Messaging;

public class TicketMessageSMSDTO
{
    public string GenderType { get; set; }
    public string UserFullName { get; set; }
    public string TicketTitle { get; set; }
    public string TrackingCode { get; set; }
    public string TicketId { get; set; }
}
