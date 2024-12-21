namespace core.application.Contract.API.DTO.Party.Resident;

public class ResidentUpdateRequest
{
    public int Id { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public bool IsHead { get; set; } = false;
    public int UserId { get; set; }
    public int UnitId { get; set; }
    public bool Renting { get; set; }
}
