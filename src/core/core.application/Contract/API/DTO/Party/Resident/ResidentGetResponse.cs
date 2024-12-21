using core.application.Contract.API.DTO.Party.User;

namespace core.application.Contract.API.DTO.Party.Resident;

public class ResidentGetResponse
{
    public int Id { get; set; }
    public string FromDate { get; set; }
    public string? ToDate { get; set; }
    public bool IsHead { get; set; } = false;
    public int UnitId { get; set; }
    public int UserId { get; set; }
    public UserGetResponseDTO User { get; set; }
    public bool Renting { get; set; }
}
