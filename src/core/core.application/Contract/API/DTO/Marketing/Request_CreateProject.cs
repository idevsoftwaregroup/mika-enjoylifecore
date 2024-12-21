using core.domain.DomainModelDTOs.MIKAMarketingDTOs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.application.Contract.API.DTO.Marketing
{
    public class Request_CreateProject
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
}
