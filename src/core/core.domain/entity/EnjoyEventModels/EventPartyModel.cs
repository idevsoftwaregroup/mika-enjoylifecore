using core.domain.entity.structureModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.domain.entity.EnjoyEventModels
{
    public class EventPartyModel
    {
        public int Id { get; set; }
        public int GuestCount { get; set; } = 0;
        //public int OwnerCount { get; set; } = 0; maybe later
        public UnitModel unit { get; set; }
        public EventModel Event  { get; set; }
    }
}
