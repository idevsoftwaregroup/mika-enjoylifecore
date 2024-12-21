using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.domain.entity.ReservationModels.JointModels
{
    public class JointDailyActivityHour
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int JointDailyActivityHourId { get; set; }
        public TimeSpan PartialStartTime { get; set; }
        public TimeSpan PartialEndTime { get; set; }
        public int JointId { get; set; }

        public virtual Joint Joint { get; set; }
    }
}
