using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.domain.DomainModelDTOs.LogDTOs.ActivityDTOs.FilterDTOs
{
    public class Filter_GetActivityLogDTO
    {
        public int? UserId { get; set; }
        public DateTime? LoginDate { get; set; }
        public DateTime? LogoutDate { get; set; }
        public int? PageNumber { get; set; } = 1;
        public int? PageSize { get; set; } = 20;
    }
}
