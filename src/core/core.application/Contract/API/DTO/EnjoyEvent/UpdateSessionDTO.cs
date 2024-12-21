using core.domain.entity.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.application.Contract.API.DTO.EnjoyEvent
{
    public class UpdateSessionDTO
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int Capacity { get; set; }
        public string Place { get; set; }
        public GenderType GenderType { get; set; }
    }
}
