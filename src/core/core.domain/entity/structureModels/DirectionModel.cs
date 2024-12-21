using core.domain.entity.enums;

namespace core.domain.entity.structureModels
{
    public class DirectionModel
    {
        public int Id { get; set; }
        public DirectionType DirectionType { get; set; }
        public DirectionModel(int id)
        {
            Id = id;
            DirectionType = DirectionType;
        }
    }
}
