namespace OrganizationChart.API.Contracts.DTOs
{
    public class CreateUpsDTO
    {
        public int ProjectId { get; set; }
        public string Location { get; set; }
        public string Model { get; set; }
        public string PowerInKVA { get; set; }
        public string? BatteryCapacityInAH { get; set; }
        public int? BatteryCount { get; set; }
        public string? MaintenanceEnvironment { get; set; }
        public DateTime PurchaseDate { get; set; }
        public DateTime? BatteryPurchaseDate { get; set; }
        public DateTime? BatteryUsageDate { get; set; }
        public string? Description { get; set; }
        public List<string>? Devices { get; set; }
    }
}
