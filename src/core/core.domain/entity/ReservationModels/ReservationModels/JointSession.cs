using core.domain.entity.enums;
using core.domain.entity.ReservationModels.JointModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.domain.entity.ReservationModels.ReservationModels
{
    public class JointSession
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long JointSessionId { get; set; }
        [MaxLength(95)]
        public string Title { get; set; }
        [MaxLength(2000)]
        public string Description { get; set; }
        public DateTime SessionDate { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public bool IsClosure { get; set; }
        public DateTime? StartReservationDate { get; set; }
        public DateTime? EndReservationDate { get; set; }
        public int? MinimumReservationMinutes { get; set; }
        public int? MaximumReservationMinutes { get; set; }
        public decimal? SessionCost { get; set; }
        public bool IsPrivate { get; set; }
        public GenderType? PublicSessionGender { get; set; }
        public bool UnitHasAccessForExtraReservation { get; set; }
        public decimal? UnitExtraReservationCost { get; set; }
        public int? Capacity { get; set; }
        public int? GuestCapacity { get; set; }
        public DateTime CreationDate { get; set; }
        public int Creator { get; set; }
        public DateTime? LastModificationDate { get; set; }
        public int? LastModifier { get; set; }
        public int JointId { get; set; }

        public virtual Joint Joint { get; set; }
        public virtual List<JointSessionAcceptableUnit> AcceptableUnits { get; set; }
        public virtual List<Reservation> Reservations { get; set; }
        public virtual List<JointSessionCancellationHour> JointSessionCancellationHours { get; set; }
    }
}
