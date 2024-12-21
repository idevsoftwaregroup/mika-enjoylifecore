using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace news.domain.Models
{
    public class Media
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [MaxLength(900)]
        public string Url { get; set; }
        public MediaType MediaType { get; set; }
        public long TempNewsArticleId { get; set; }


        public virtual TempNewsArticle TempNewsArticle { get; set; }
    }
}
