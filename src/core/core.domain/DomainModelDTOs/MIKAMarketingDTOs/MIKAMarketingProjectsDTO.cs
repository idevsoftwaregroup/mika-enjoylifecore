using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

public class MIKAMarketingProjectsDTO
{
    [Key]
    public int Id { get; set; }
    public string ProjectName { get; set; }
    public string ProjectDescription { get; set; }
    public string ProjectType { get; set; }
    public string ProjectLocation { get; set; }
    public string ProjectFloors { get; set; }
    public string ProjectBlocks { get; set; }
    [AllowNull]
    public string ProjectBlocksDetails { get; set; }
    public DateTime ProjectDateTimeRegistration { get; set; } = DateTime.Now;
    public string? ProjectDateTimeDelivery { get; set; }
    public decimal ProjectBasePrice { get; set; }
    public decimal ProjectMultiplyProjectPerBlock { get; set; }
    public string ProjectUnits { get; set; }
    public string ProjectBuildingMeterages { get; set; }
    [AllowNull]
    public string ProjectUnitsDetails { get; set; } 
    [AllowNull]
    public bool? IsDeleted { get; set; } = false;
}
