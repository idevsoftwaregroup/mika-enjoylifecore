using common.defination;
using core.domain.entity.structureModels;

namespace core.domain.entity.partyModels
{
    public class ResidentModel : BaseEntity
    {
        public int Id { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public bool IsHead { get; set; } = false;
        public UnitModel Unit { get; set; }
        public UserModel User { get; set; }
        public bool Renting { get; set; }
    }
}
