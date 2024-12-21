using core.domain.DomainModelDTOs.LogDTOs.CommonDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.domain.DomainModelDTOs.LogDTOs.ActionDTOs
{
    public class GetActionLogDTO
    {
        public PageResponseDTO PaginationData { get; set; }
        public List<ActionLogSingleDTO> ListData { get; set; }
    }
    public class ActionLogSingleDTO
    {
        public long Id { get; set; }
        public int UserId { get; set; }
        public DateTime LoginDate { get; set; }
        public DateTime? LogoutDate { get; set; }
        public int ModuleId { get; set; }
        public string ModuleName { get; set; }
        public string ModuleDisplayName { get; set; }
        public string ActionName { get; set; }
        public string? ActionDescription { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public DateTime LogDate { get; set; }
    }
}
