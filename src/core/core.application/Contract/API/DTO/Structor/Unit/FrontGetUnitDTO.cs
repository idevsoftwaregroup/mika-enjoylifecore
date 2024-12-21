using core.domain.entity.enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.application.Contract.API.DTO.Structor.Unit
{
    public class FrontGetUnitDTO
    {
        public int Id { get; set; }
        [MaxLength(20), MinLength(2)]
        public string Name { get; set; } = null!;
        [Range(2.00, 1000.00)]
        public decimal Meterage { get; set; }
        public UnitUsageType UnitUsages { get; set; }
        public List<DirectionType> Directions { get; set; }
        public List<DirectionType> Positions { get; set; }
        public List<string> PositionsDescription { get; set; }
        public UnitType Type { get; set; }
        public int Floor { get; set; }
        public int ComplexId { get; set; }
        public int Block { get; set; }
        public string UnitElectricityCounterNumber { get; set; }
        //public int TotalParkingLots { get; set; }
        public List<string> Parkings { get; set; }
        //public int TotalStorageLots { get; set; }
        public List<string> StorageLots { get; set; }

        public string UnitPLanMapFileUrl { get; set; }
    }
}
