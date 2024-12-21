namespace core.application.Contract.API.DTO.EnjoyEvent;

public class CreateReservationRequestDTO
{
    public int UserId { get; set; }
    public int UnitId { get; set; }
    public long? SessionId { get; set; }
    public int? EventId { get; set; }

}
