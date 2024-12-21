using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.domain.entity.EnjoyEventModels
{
    public class EventContentModel

    {
        public int Id { get; set; }
        public string? ContentBody { get; set; } = string.Empty;
        public EventModel Event { get; set; }
        public BodyType BodyType { get; set; }
        public List<EventMediaModel> Media { get; set; }
    }
    public enum BodyType
    {
        THUMBNAIL,
        COMMINGSOON,
        ONGOING,
        ARCHIVED
    }

}
