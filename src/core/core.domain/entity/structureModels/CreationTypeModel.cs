namespace core.domain.entity.structureModels
{
    public class CreationTypeModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        /// <summary>
        /// 1) unit
        /// 2) parking
        /// 3) warehouse
        /// </summary>
        public int TypeId { get; set; }
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public string Description { get; set; }
    }
}
