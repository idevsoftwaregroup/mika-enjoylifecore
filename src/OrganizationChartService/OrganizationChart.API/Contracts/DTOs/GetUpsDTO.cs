namespace OrganizationChart.API.Contracts.DTOs
{
    public class GetUpsDTO
    {
        public int Id { get; set; }
        public string Location { get; set; }
        public string Model { get; set; }
        public string PowerInKVA { get; set; }
        public string? BatteryCapacityInAH { get; set; }
        public int? BatteryCount { get; set; }
        public string? MaintenanceEnvironment { get; set; }
        public DateTime PurchaseDate { get; set; }
        public DateTime? BatteryPurchaseDate { get; set; }
        public DateTime? BatteryUsageDate { get; set; }
        public DateTime? LatestServiceDate { get; set; }
        public string? Description { get; set; }
        public List<string>? Devices { get; set; }
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public List<GetUps_ServicesDTO>? Services { get; set; }
    }
    public class GetUps_ServicesDTO
    {
        public long ServiceId { get; set; }
        public DateTime ServiceDate { get; set; }
        public string? ServiceDescription { get; set; }
        public string? AttachmentUrl { get; set; }
    }
}
