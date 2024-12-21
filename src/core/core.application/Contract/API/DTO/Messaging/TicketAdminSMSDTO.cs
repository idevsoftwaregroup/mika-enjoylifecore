namespace core.application.Contract.API.DTO.Messaging;

public class TicketAdminSMSDTO
{
    public string Title { get; set; }
    public string Description { get; set; }
    public string CreatedBy { get; set; }
    public string UnitName { get; set; }
    public string AttchmentUrl { get; set; } = String.Empty;
    public string CreateDate { get; set; }
}
