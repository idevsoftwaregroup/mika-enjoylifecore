using news.domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace news.application.DomainModelDTOs
{
    public class PostTempNewsArticleDomainDTO
    {
        public string Title { get; set; }
        public string? Description { get; set; }
        public NewsTagType NewsTag { get; set; }
        public bool? Important { get; set; }
        public bool? Pinned { get; set; }
        public bool? Active { get; set; }
        public long? Priority { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public MediaDTO? Thumbnail { get; set; }
        public List<MediaDTO> Medias { get; set; }
    }

}
