using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.domain.DomainModelDTOs.WarehouseDTOs
{
    public class ItemList
    {
        [Key]
        public int ItemListCode { get; set; }
        public string Group { get; set; }
        public string Name { get; set; }
        public string Unit { get; set; }
    }
}
