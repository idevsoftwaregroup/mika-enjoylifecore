using core.domain.entity.structureModels;

namespace core.domain.entity.EnjoyEventModels;

public class EventTicketModel
{
    public long Id { get; set; }
    public UnitModel Unit { get; set; }
    public UserModel User { get; set; }
    public EventSessionModel Session { get; set; }
    public int MaleNum { get; set; }
    public int FemaleNum { get; set; }
    public int GuestMaleNum { get; set; } = 0;
    public int GuestFemaleNum { get; set; } = 0;
    public DateTime CreatedAt { get; set; }
    public DateTime LastModified { get; set; }
}
