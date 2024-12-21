namespace OrganizationChart.API.Contracts.DTOs
{
    public class UpdateSystemDetailsDTO
    {
        public long SystemId { get; set; }
        public string? ComputerName { get; set; }
        public int? SystemTypeId { get; set; }
        public string? User { get; set; }
        public string? UserPicUrl { get; set; }
        public string? OS { get; set; }
        public string? RAM { get; set; }
        public string? MainBoardName { get; set; }
        public string? MainBoardModel { get; set; }
        public string? CPU { get; set; }
        public List<DiskListSystemDTO>? HardDisks { get; set; }
        public List<GraphicSystemDTO>? GraphicCards { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }
        public string? SystemPicUrl { get; set; }
    }
  
}
