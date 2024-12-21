using news.domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace news.application.DomainModelDTOs.FilterDTOs
{
    public class Customer_TempNewsArticle_FilterDTO
    {
        public string? Search { get; set; }
        public List<int>? NewsTags { get; set; }
        public bool? Important { get; set; }
        public DateTime? CreationDate { get; set; }
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
    }
}
