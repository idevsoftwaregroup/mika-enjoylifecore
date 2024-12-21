using news.domain.Models.Aggregates.NewsArticle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace news.application.Contracts.DTOs.NewsArticleDTOs
{
    public class NewsArticleDTO
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string? TextContent { get; set; }

        public DateTime PublishDate { get; set; }
        public MultiMediaContentDTO? Thumbnail { get; set; }

        public Guid BuildingId { get; set; }

        public NewsTagTypesDTO NewsTagTypes { get; set; }

        public IEnumerable<Guid> ViewerIds { get; set; }

        public IEnumerable<MultiMediaContentDTO> MultiMediaContentDTOs { get; set; }
        
        public static explicit operator NewsArticleDTO(NewsArticle? newsArticle)
        {
            List<Guid> ids = new List<Guid>();
            foreach (var item in newsArticle.Viewers)
            {
                ids.Add(item);
            }

            return new NewsArticleDTO()
            {
                Id = newsArticle.Id,
                Title = newsArticle.Title,
                Description = newsArticle.Description,
                TextContent = newsArticle.TextContent,
                PublishDate = newsArticle.PublishDate,
                Thumbnail = (MultiMediaContentDTO?)newsArticle.Thumbnail,
                BuildingId = newsArticle.BuildingId,
                NewsTagTypes = (NewsTagTypesDTO)newsArticle.NewsTagTypes,
                ViewerIds = ids,
                MultiMediaContentDTOs = newsArticle.MultiMediaContents.Cast<MultiMediaContentDTO>(), //possibly should change

            };
        }
    }
}
