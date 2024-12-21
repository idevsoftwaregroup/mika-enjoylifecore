using core.domain.entity.EnjoyEventModels;

namespace core.application.Contract.API.DTO.EnjoyEvent;
public class UpdateEventContentDTO
{
    public string? ContentBody { get; set; }
    public BodyType BodyType { get; set; }
    
}
