namespace core.application.Contract.API.DTO.Party.Manager;

public class ManagerGetResponse
{
    public int Id { get; set; }
    public string FromDate { get; set; }
    public string? ToDate { get; set; }
    public int UserId { get; set; }
    public int ComplexId { get; set; }

   // public string PersianFromDate { get; set; }
   // public string? PersianToDate { get; set; }
}
