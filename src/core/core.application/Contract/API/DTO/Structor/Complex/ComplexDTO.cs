using core.domain.entity.enums;
using core.domain.entity.validationAttributes;
using System.ComponentModel.DataAnnotations;

namespace core.application.Contract.API.DTO.Structor.Complex
{
    public class ComplexDTO
    {
        [MaxLength(20), MinLength(2)]
        public string Title { get; set; } = null!;

        [MinLength(5)]
        public string Address { get; set; } = null!;

        public string? Description { get; set; }

        [DirectionNotNONE(ErrorMessage = "choose atleast one direction")]
        public DirectionType Directions { get; set; } = DirectionType.NONE;

        [DirectionNotNONE(ErrorMessage = "choose atleast one position")]
        public DirectionType Positions { get; set; } = DirectionType.NONE;

        [UnitUsageNotNONE(ErrorMessage = "choose atleast one unit usage")]
        public UnitUsageType UnitUsages { get; set; } = UnitUsageType.NONE;

        // public List<ImageModel> Images { get; set; }

        // public DocumentModel JointsDocument { get; set; }

        //public ImageModel Logo { get; set; }

        // public PaymentPortModel PaymentPort { get; set; }
        //floor counts
        // public List<ImageModel> Images { get; set; }
        //public List<ManagerDTO>? Managers { get; set; }
        // public DocumentModel JointsDocument { get; set; }

        //public ImageModel Logo { get; set; }

        // public PaymentPortModel PaymentPort { get; set; }

        //public decimal? Meterage { get; set; }
    }
}
