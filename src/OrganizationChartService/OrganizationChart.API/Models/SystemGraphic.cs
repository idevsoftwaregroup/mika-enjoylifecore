using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace OrganizationChart.API.Models
{
    public class SystemGraphic
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [MaxLength(300)]
        public string GraphicCardName { get; set; }
        public long SystemId { get; set; }

        public virtual Systems System { get; set; }
    }
}
