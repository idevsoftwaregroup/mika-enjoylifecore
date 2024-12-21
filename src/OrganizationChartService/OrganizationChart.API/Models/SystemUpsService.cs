using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace OrganizationChart.API.Models
{
    public class SystemUpsService
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public DateTime Date { get; set; }
        [MaxLength(2000)]
        public string Description { get; set; }
        [MaxLength(200)]
        public string AttachmentUrl { get; set; }
        public int UpsId { get; set; }

        public virtual SystemUPS UPS { get; set; }
    }
}
