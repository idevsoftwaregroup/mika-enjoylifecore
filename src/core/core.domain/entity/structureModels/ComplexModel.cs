using common.defination;
using core.domain.entity.enums;
using core.domain.entity.ReservationModels.JointModels;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace core.domain.entity.structureModels
{
    public class ComplexModel : BaseEntity
    {
        public int Id { get; set; }
        [MaxLength(20)]         
        public string Title { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string? Description { get; set; }
        public DirectionType Directions { get; set; }
        public DirectionType Positions { get; set; }
        public ComplexUsageType Usages { get; set; }

        public virtual List<Joint> Joints { get; set; }
    }
}
