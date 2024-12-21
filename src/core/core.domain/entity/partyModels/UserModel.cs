using common.defination;
using core.domain.entity.enums;
using core.domain.entity.partyModels;
using core.domain.entity.ReservationModels.ReservationModels;
using core.domain.entity.ticketingModels;
using core.domain.entity.TicketModels;
using core.domain.entity.WebSocketModels;

namespace core.domain.entity.structureModels
{
    public class UserModel : BaseEntity
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Email { get; set; }
        public string PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public string? NationalID { get; set; }
        public string? Address { get; set; }
        public int? Age { get; set; }
        public GenderType? Gender { get; set; }

        public virtual List<UserConnectionModel> UserConnections { get; set; }
        public virtual List<TicketMessageModel> TicketMessages { get; set; }
        public virtual List<TicketModel> Tickets { get; set; }
        public virtual List<TicketModel> TicketsTechnicians { get; set; }
        public virtual List<TicketSeenModel> TicketsSeens { get; set; }
        public virtual List<UserLookUpModel> LookUps { get; set; }
        public virtual List<Reservation> Reservations { get; set; }
    }
}
