namespace OrganizationChart.API.Contracts.DTOs
{
    public class CreateUpsServiceDTO
    {
        public int UpsId { get; set; }
        public DateTime? Date { get; set; }
        public string? Description { get; set; }
        public string? AttachmentUrl { get; set; }

    }
}
