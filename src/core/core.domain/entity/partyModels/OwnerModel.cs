using common.defination;
using core.domain.entity.structureModels;

namespace core.domain.entity.partyModels
{
    public class OwnerModel : BaseEntity
    {
        public int Id { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int Percentage { get; set; }
        public UnitModel Unit { get; set; }
        public UserModel User { get; set; }
    }
}
