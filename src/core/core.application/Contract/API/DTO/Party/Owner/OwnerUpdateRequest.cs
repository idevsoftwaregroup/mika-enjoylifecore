namespace core.application.Contract.API.DTO.Party.Owner;

public class OwnerUpdateRequest
{
    public int Id { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public int Percentage { get; set; }
    public int UnitId { get; set; }
    public int UserId { get; set; }
}
