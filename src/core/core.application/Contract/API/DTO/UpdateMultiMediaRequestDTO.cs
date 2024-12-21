using core.domain.entity;

namespace core.application.Contract.API.DTO;

public class UpdateMultiMediaRequestDTO
{
    public string Url { get; set; }
    public MultiMediaType MediaType { get; set; }
    public string Alt { get; set; }
}
