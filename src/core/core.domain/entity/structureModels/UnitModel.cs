using common.defination;
using core.domain.entity.enums;
using core.domain.entity.financialModels;
using core.domain.entity.partyModels;
using core.domain.entity.ReservationModels.ReservationModels;
using core.domain.entity.TicketModels;
using System.ComponentModel.DataAnnotations;

namespace core.domain.entity.structureModels
{
    public class UnitModel : BaseEntity
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
        public DirectionType Directions { get; set; }
        public DirectionType Positions { get; set; }
        public UnitType Type { get; set; }
        public string UnitElectricityCounterNumber { get; set; }
        public List<ResidentModel>? Residents { get; set; }
        public List<OwnerModel>? Owners { get; set; }
        //public int TotalParkingLots { get; set; }
        //public int TotalStorageLots { get; set; }
        public List<StorageLot> StorageLots { get; set; } = new();
        public List<Parking> Parkings { get; set; } = new();
        public virtual List<TicketModel> Tickets { get; set; }
        public string UnitPLanMapFileUrl { get; set; }
        public virtual List<JointSessionAcceptableUnit> AcceptableUnits { get; set; }
        public virtual List<Reservation> Reservations { get; set; }
        public virtual List<BillModel> Bills { get; set; }
    }

    public class Parking
    {
        public int Id { get; set;}
        public string Name { get; set; }
   
    }

    public class StorageLot
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
