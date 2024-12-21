using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace OrganizationChart.API.Models
{
    public class SystemUPS
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [MaxLength(250)]
        public string Location { get; set; }
        [MaxLength(210)]
        public string Model { get; set; }
        [MaxLength(35)]
        public string PowerInKVA { get; set; }
        [MaxLength(3000)]
        public string Devices { get; set; }
        [MaxLength(35)]
        public string BatteryCapacityInAH { get; set; }
        public int? BatteryCount { get; set; }
        [MaxLength(210)]
        public string MaintenanceEnvironment { get; set; }
        public DateTime PurchaseDate { get; set; }
        public DateTime? BatteryPurchaseDate { get; set; }
        public DateTime? BatteryUsageDate { get; set; }
        public DateTime? LatestServiceDate { get; set; }
        [MaxLength(2000)]
        public string Description { get; set; }
        public int ProjectId { get; set; }

        public virtual SystemProject Project { get; set; }
        public virtual List<SystemUpsService> UpsServices { get; set; }
    }
}
