using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.domain.entity.ReservationModels.ReservationModels
{
    public class JointSessionCancellationHour
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long JointSessionCancellationHourId { get; set; }
        public int Hour { get; set; }
        public long JointSessionId { get; set; }

        public virtual JointSession JointSession { get; set; }
    }
}
