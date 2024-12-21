using core.domain.entity.enums;
using System.ComponentModel.DataAnnotations;

namespace core.application.Contract.API.DTO.Structor.Unit;

public class UnitUpdateRequestDTO
{
    public int Id { get; set; }
    [MaxLength(20)]
    public string Name { get; set; }
    public int Floor { get; set; }
    public int ComplexId { get; set; }
    public int Block { get; set; }
    [Range(2.00, 1000.00)]
    public decimal Meterage { get; set; }
    public UnitUsageType UnitUsages { get; set; }
    public List<DirectionType> Directions { get; set; }
    public List<DirectionType> Positions { get; set; }
    public UnitType Type { get; set; }
    public string UnitElectricityCounterNumber { get; set; }

    //public int TotalParkingLot { get; set; }
    //public int TotalStorageLot { get; set; }
    public string UnitPLanMapFileUrl { get; set; }
}
