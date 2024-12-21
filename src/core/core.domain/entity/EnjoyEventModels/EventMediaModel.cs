using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.domain.entity.EnjoyEventModels
{
    public class EventMediaModel
    {
        public int Id { get; set; }
        public string Url { get; set; }       
        public string Alt { get; set; }
        public MediaType Type { get; set; }       
    }

    public enum MediaType
    {
        IMAGE,
        VIDEO,
        GIF
    }

}
