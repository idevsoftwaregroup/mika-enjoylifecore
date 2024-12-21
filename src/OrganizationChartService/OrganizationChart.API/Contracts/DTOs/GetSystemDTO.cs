using System.ComponentModel.DataAnnotations;

namespace OrganizationChart.API.Contracts.DTOs
{
    public class GetSystemDTO
    {
        public long SystemId { get; set; }
        public string ComputerName { get; set; }
        public string User { get; set; }
        public string OS { get; set; }
        public string RAM { get; set; }
        public string MainBoardName { get; set; }
        public string MainBoardModel { get; set; }
        public string CPU { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }
        public string? SystemPicUrl { get; set; }
        public string? UserPicUrl { get; set; }
        public int SystemTypeId { get; set; }
        public string SystemTypeName { get; set; }

        public List<GetSystem_DiskDTO> Disks { get; set; }
        public List<GetSystem_GraphicDTO> Graphics { get; set; }
    }

    public class GetSystem_DiskDTO
    {
        public int DiskId { get; set; }
        public string DiskName { get; set; }
        public string DiskVolume { get; set; }
    }
    public class GetSystem_GraphicDTO
    {
        public int GraphicCardId { get; set; }
        public string GraphicCardName { get; set; }
    }
}
