namespace OrganizationChart.API.Contracts.DTOs
{
    public class UpdateUpsServiceDTO
    {
        public long UpsServiceId { get; set; }
        public string? Description { get; set; }
        public string? AttachmentUrl { get; set; }
    }
}
