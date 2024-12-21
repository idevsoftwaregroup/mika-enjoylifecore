using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using core.domain.entity.structureModels;

namespace core.domain.entity.ReservationModels.ReservationModels
{
    public class Reservation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ReservationId { get; set; }
        public long JointSessionId { get; set; }
        public TimeSpan StartDate { get; set; }
        public TimeSpan EndDate { get; set; }
        public int Count { get; set; }
        public int GuestCount { get; set; }
        public decimal Cost { get; set; }
        public bool CancelledByAdmin { get; set; }
        public bool CancelledByByUser { get; set; }
        [MaxLength(2000)]
        public string CancellationDescription { get; set; }
        public int ReservedById { get; set; }
        public int ReservedForUnitId { get; set; }
        public DateTime ReservationDate { get; set; }
        public int? LastModifier { get; set; }
        public DateTime? LastModificationDate { get; set; }

        public virtual JointSession JointSession { get; set; }
        public virtual UserModel ReservedBy { get; set; }
        public virtual UnitModel ReservedForUnit { get; set; }
    }
}
