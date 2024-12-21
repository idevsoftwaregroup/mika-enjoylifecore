using core.domain.entity.EnjoyEventModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.application.Contract.API.DTO.EnjoyEvent
{
    public class EventMediaRequestDTO
    {
        public string Url { get; set; }
        public string Alt { get; set; }
        public MediaType Type { get; set; }
    }
}
