using core.domain.entity;

namespace core.application.Contract.API.DTO;

public class MultiMediaDTO
{
    public long? Id { get; set; }
    public string Url { get; set; }
    public MultiMediaType MediaType { get; set; }
    public string Alt { get; set; }
}
