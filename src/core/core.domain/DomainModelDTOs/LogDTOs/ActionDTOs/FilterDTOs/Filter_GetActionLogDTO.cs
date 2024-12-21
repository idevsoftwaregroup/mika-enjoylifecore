using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.domain.DomainModelDTOs.LogDTOs.ActionDTOs.FilterDTOs
{
    public class Filter_GetActionLogDTO
    {
        public int? UserId { get; set; }
        public DateTime? LoginDate { get; set; }
        public DateTime? LogoutDate { get; set; }
        public int? ModuleId { get; set; }
        public string? Search { get; set; }
        public string? Key { get; set; }
        public DateTime? LogDate { get; set; }
        public int? PageNumber { get; set; } = 1;
        public int? PageSize { get; set; } = 20;
    }
}
