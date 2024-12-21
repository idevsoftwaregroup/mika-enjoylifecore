using core.domain.entity.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.domain.DomainModelDTOs.Units_Residents_OwnersDTOs.FilterDTOs
{
    public class Filter_GetUnitOwnersResidentsDomainDTO
    {
        //unit filter
        public int? UnitId { get; set; }
        public string? UnitName { get; set; }
        public int? Floor { get; set; }

        //user (owner and resident) filter
        public int? UserId { get; set; }
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public GenderType? Gender { get; set; }
        public bool? IsHeadResident { get; set; }
        public bool? RentingResident { get; set; }

        //pagination
        public int? PageNumber { get; set; } = 1;
        public int? PageSize { get; set; } = 20;
    }
}
