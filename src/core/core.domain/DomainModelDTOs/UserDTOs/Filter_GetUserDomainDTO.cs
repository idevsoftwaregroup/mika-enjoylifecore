using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.domain.DomainModelDTOs.UserDTOs
{
    public class Filter_GetUserDomainDTO
    {
        public int? UserId { get; set; }
        public int? UnitId { get; set; }
        public string? PhoneNumber { get; set; }
        public int? PageNumber { get; set; } = 1;
        public int? PageSize { get; set; } = 20;
    }
}
