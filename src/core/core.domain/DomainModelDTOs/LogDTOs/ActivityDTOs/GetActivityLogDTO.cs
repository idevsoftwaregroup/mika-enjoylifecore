using core.domain.DomainModelDTOs.LogDTOs.CommonDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.domain.DomainModelDTOs.LogDTOs.ActivityDTOs
{
    public class GetActivityLogDTO
    {
        public PageResponseDTO PaginationData { get; set; }
        public List<ActivityLogSingleDTO> ListData { get; set; }
    }
    public class ActivityLogSingleDTO
    {
        public long Id { get; set; }
        public int UserId { get; set; }
        public DateTime LoginDate { get; set; }
        public DateTime? LogoutDate { get; set; }
    }
}
