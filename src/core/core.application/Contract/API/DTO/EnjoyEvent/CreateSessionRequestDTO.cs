using core.domain.entity.enums;

namespace core.application.Contract.API.DTO.EnjoyEvent;

public class CreateSessionRequestDTO
{
    public int EventId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int Capacity { get; set; }
    public GenderType GenderType { get; set; }
    public string Place { get; set; }
    public bool IsActive { get; set; }
}
