using core.domain.DomainModelDTOs.MIKAMarketingDTOs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.application.Contract.API.DTO.Marketing
{
    public class Request_UpdateProject
    {
        [Key]
        public int Id { get; set; }
        public string ProjectName { get; set; }
        public string ProjectDescription { get; set; }
        public string ProjectType { get; set; }
        public string ProjectLocation { get; set; }
        public string ProjectFloor { get; set; }
        public string ProjectBlocks { get; set; }
        public string ProjectDateTimeDelivery { get; set; }
        public decimal ProjectBasePrice { get; set; }
        public decimal ProjectMultiplyProjectPerBlock { get; set; }

        // لیست‌های مختلف
        // Nested lists for units, files, terraces, etc.
        public string ProjectUnits { get; set; }
        public string ProjectBuildingMeterages { get; set; }
        public bool? IsDeleted { get; set; } = false;
    }
}
