using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.domain.entity.ReservationModels.JointModels
{
    public class JointStatus
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int JointStatusId { get; set; }
        [MaxLength(150)]
        public string Title { get; set; }
        [MaxLength(300)]
        public string DisplayTitle { get; set; }

        public virtual List<Joint> Joints { get; set; }
    }
}
