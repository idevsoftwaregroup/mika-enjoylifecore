namespace OrganizationChart.API.Contracts.DTOs
{
    public class GetSystemDetailsResponse
    {
        // Details :
        public int Id { get; set; }
        public string SystemId { get; set; }
        public string SystemName { get; set; }
        public string Description { get; set; }
        public string SystemPicUrl { get; set; }
        public string Location { get; set; }
    }
}
