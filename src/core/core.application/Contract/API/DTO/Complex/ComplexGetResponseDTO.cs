using core.domain.entity.enums;
using System.ComponentModel.DataAnnotations;

namespace core.application.Contract.API.DTO.Complex;

public class ComplexGetResponseDTO
{
    public int Id { get; set; }
    [MaxLength(20), MinLength(2)]
    public string Title { get; set; } = null!;

    [MinLength(5)]
    public string Address { get; set; } = null!;

    public string? Description { get; set; }

    [ListMustHaveValue(ErrorMessage = "choose atleast one direction")]

    public List<DirectionType> Directions { get; set; }

    [ListMustHaveValue(ErrorMessage = "choose atleast one position")]
    public List<DirectionType> Positions { get; set; }


    [ListMustHaveValue(ErrorMessage = "choose atleast one position")]
    public List<ComplexUsageType> Usages { get; set; }
}
