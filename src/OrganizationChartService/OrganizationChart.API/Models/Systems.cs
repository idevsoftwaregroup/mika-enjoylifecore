using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace OrganizationChart.API.Models
{
    public class Systems
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [MaxLength(350)]
        public string ComputerName { get; set; }
        [MaxLength(350)]
        public string User { get; set; }
        [MaxLength(700)]
        public string OS { get; set; }
        [MaxLength(45)]
        public string RAM { get; set; }
        [MaxLength(350)]
        public string MainBoardName { get; set; }
        [MaxLength(350)]
        public string MainBoardModel { get; set; }
        [MaxLength(350)]
        public string CPU { get; set; }
        [MaxLength(1500)]
        public string Description { get; set; }
        [MaxLength(200)]
        public string Location { get; set; }
        [MaxLength(200)]
        public string SystemPicUrl { get; set; }
        [MaxLength(200)]
        public string UserPicUrl { get; set; }
        public int SystemTypeId { get; set; }


        public virtual SystemType Type { get; set; }
        public virtual List<SystemHard> HardDisks { get; set; }
        public virtual List<SystemGraphic> Graphics { get; set; }

    }
}
