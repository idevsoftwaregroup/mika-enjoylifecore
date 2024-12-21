using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace OrganizationChart.API.Models
{
    public class SystemHard
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [MaxLength(300)]
        public string DiskName { get; set; }
        [MaxLength(55)]
        public string DiskVolume { get; set; }
        public long SystemId { get; set; }

        public virtual Systems System { get; set; }
    }
}
