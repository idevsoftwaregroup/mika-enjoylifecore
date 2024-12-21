namespace core.application.Contract.API.DTO.Party.Resident;

public class ResidentGetRequestFilter
{
    public int? UserId { get; set; }
    public int? UnitId { get; set; }
    public bool? Renting { get; set; }
    public bool? Head { get; set; }

}
