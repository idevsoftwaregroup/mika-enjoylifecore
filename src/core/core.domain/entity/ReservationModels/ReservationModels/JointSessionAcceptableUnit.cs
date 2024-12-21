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
    public class JointSessionAcceptableUnit
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long JointSessionAcceptableUnitId { get; set; }
        public long JointSessionId { get; set; }
        public int UnitId { get; set; }

        public virtual JointSession JointSession { get; set; }
        public virtual UnitModel Unit { get; set; }
    }
}
