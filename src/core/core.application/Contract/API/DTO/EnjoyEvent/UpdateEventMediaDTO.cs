using core.domain.entity.EnjoyEventModels;

namespace core.application.Contract.API.DTO.EnjoyEvent;

public class UpdateEventMediaDTO
{
    public string Url { get; set; }
    public string Alt { get; set; }
    public MediaType Type { get; set; }
}
