using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.domain.DomainModelDTOs.WarehouseDTOs
{
    public class EnjoylifeItems
    {
        [Key]
        public int Id { get; set; }
        public int ItemCode { get; set; }
        public string ItemName { get; set; }
        public string ItemDescription { get; set; }
        public int ItemCounts { get; set; }
        public bool ItemStatus { get; set; }
        public string ItemDateRegistration { get; set; }
        public string ItemGroup { get; set; }
        public string ItemUnit { get; set; }
        public string ItemMonth { get; set; }
    }


}
