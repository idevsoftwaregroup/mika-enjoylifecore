using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.application.Contract.API.DTO.Concierge
{
    public class ConciergeResponseDTO
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? SupportTitle { get; set; }
        public string SupportDescription { get; set; } = string.Empty;
        public string SupportPhone { get; set; } = string.Empty;
    }
}
