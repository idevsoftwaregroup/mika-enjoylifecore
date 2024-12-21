using core.application.Contract.API.DTO.Party.User;

namespace core.application.Contract.API.DTO.Party.Owner;

public class OwnerGetResponse
{
    public int Id { get; set; }
    public string FromDate { get; set; }
    public string ToDate { get; set; }
    public int Percentage { get; set; }
    public int UnitID { get; set; }
    public int UserID { get; set; }
    public UserGetResponseDTO User { get; set; }

}
