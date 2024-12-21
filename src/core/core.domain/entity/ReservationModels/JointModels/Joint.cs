using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using core.domain.entity.structureModels;
using core.domain.entity.ReservationModels.ReservationModels;

namespace core.domain.entity.ReservationModels.JointModels
{
    public class Joint
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int JointId { get; set; }
        [MaxLength(110)]
        public string Title { get; set; }
        [MaxLength(180)]
        public string Location { get; set; }
        [MaxLength(1000)]
        public string PhoneNumbers { get; set; }
        [MaxLength(2500)]
        public string Description { get; set; }
        [MaxLength(2000)]
        public string TermsText { get; set; }
        [MaxLength(200)]
        public string TermsFileUrl { get; set; }
        [MaxLength(200)]
        public string ThumbnailUrl { get; set; }
        public int? DailyUnitReservationCount { get; set; }
        public int? WeeklyUnitReservationCount { get; set; }
        public int? MonthlyUnitReservationCount { get; set; }
        public int? YearlyUnitReservationCount { get; set; }
        public int ComplexId { get; set; }
        public int JointStatusId { get; set; }

        public virtual ComplexModel Complex { get; set; }
        public virtual JointStatus JointStatus { get; set; }
        public virtual List<JointMultiMedia> JointMultiMedias { get; set; }
        public virtual List<JointDailyActivityHour> JointDailyActivityHours { get; set; }
        public virtual List<JointSession> JointSessions { get; set; }
    }
}
