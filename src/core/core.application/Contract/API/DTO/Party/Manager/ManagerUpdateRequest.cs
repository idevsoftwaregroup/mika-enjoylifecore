namespace core.application.Contract.API.DTO.Party.Manager;

public class ManagerUpdateRequest
{
    public int Id { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int UserId { get; set; }
    public int ComplexId { get; set; }
}
