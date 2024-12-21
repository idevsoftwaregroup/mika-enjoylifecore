using core.domain.entity.EnjoyEventModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.application.Contract.API.DTO.EnjoyEvent
{
    public class CreateEventContentRequestDTO
    {
        public int EventId { get; set; }
        public string? ContentBody { get; set; } = string.Empty;
        public BodyType BodyType { get; set; }
        public List<EventMediaRequestDTO> Media { get; set; }
    }
}
