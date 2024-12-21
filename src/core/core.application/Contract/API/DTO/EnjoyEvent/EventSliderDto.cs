using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.application.Contract.API.DTO.EnjoyEvent
{
    public class EventSliderDto
    {
        public int Id { get; set; }
        public int? Type { get; set; }
        public string? src { get; set; }
        public string? alt { get; set; }
    }
}
