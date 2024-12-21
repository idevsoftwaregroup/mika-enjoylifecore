using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.domain.DomainModelDTOs.LogDTOs.ActionDTOs
{
    public class SetLogDTO
    {
        public string Module { get; set; }
        public string ActionName { get; set; }
        public string? Description { get; set; }
        public string Key { get; set; }
        public string? Value { get; set; }
    }
}
