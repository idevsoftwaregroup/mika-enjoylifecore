using core.domain.entity.EnjoyEventModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.domain.entity.ReservationModels.JointModels
{
    public class JointMultiMedia
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long JointMultiMediaId { get; set; }
        [MaxLength(200)]
        public string Url { get; set; }
        public MediaType MediaType { get; set; }
        [MaxLength(250)]
        public string Alt { get; set; }
        public int JointId { get; set; }

        public virtual Joint Joint { get; set; }
    }
}
