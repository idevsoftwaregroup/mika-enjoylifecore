using common.defination;
using core.domain.entity.structureModels;

namespace core.domain.entity.partyModels
{
    public class ManagerModel : BaseEntity
    {
        public int Id { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public UserModel User { get; set; }
        public ComplexModel Complex { get; set; }
    }
}
