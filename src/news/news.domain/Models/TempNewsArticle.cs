using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace news.domain.Models
{
    public class TempNewsArticle
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [MaxLength(300)]
        public string Title { get; set; }
        [MaxLength(2000)]
        public string Description { get; set; }
        public NewsTagType NewsTag { get; set; }
        public bool Important { get; set; }
        public bool Pinned { get; set; }
        public bool Active { get; set; }
        public long Priority { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        [MaxLength(900)]
        public string ThumbnailUrl { get; set; }
        public MediaType? ThumbnailMediaType { get; set; }
        public DateTime CreationDate { get; set; }
        public int Creator { get; set; }
        public DateTime? ModificationDate { get; set; }
        public int? Modifier { get; set; }


        public virtual List<Media> Medias { get; set; }       
    }
}
