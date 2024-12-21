using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.application.Contract.API.DTO.EnjoyEvent
{
    public class SetEventPartyDTO
    {
        public int EventId { get; set; }
        public int UnitId { get; set; }  
        public int MaxGuestCount { get; set; }
    }
}
