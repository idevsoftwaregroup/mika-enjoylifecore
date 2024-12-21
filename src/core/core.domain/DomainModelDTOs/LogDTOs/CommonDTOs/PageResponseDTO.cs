using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.domain.DomainModelDTOs.LogDTOs.CommonDTOs
{
    public class PageResponseDTO
    {
        public long Count { get; set; }
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
    }
}
