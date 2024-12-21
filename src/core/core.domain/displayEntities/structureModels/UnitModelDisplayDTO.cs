using core.domain.entity.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.domain.displayEntities.structureModels
{
    public class UnitModelDisplayDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Floor { get; set; }
        public int ComplexId { get; set; }
        public int Block { get; set; }
        public decimal Meterage { get; set; }
        public UnitUsageType UnitUsages { get; set; }
        public DirectionType Directions { get; set; }
        public DirectionType Positions { get; set; }
        public UnitType Type { get; set; }
        public string UnitElectricityCounterNumber { get; set; }
        public string UnitPLanMapFileUrl { get; set; }

    }
}
