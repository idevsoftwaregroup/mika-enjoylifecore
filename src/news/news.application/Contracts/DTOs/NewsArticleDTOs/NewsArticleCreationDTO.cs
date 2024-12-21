using news.domain.Models.Aggregates.NewsArticle;
using news.domain.Models.Aggregates.NewsArticle.ValueObjects.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace news.application.Contracts.DTOs.NewsArticleDTOs
{
    public class NewsArticleCreationDTO
    {
        public string Title {  get; set; }
        public string Description { get; set; }
        public DateTime PublishDate { get; set; }
        public Guid BuildingId { get; set; }
        public NewsTagTypeCreationDTO NewsTagTypeCreationDTO { get; set; }

        #region optionals

        public MultiMediaContentDTO? Thumbnail { get; set; }
        public IEnumerable<MultiMediaContentDTO>? MultiMediaContentDTOs { get; set; } = new List<MultiMediaContentDTO>();
        public string? TextContent { get; set; }
        #endregion

        public static implicit operator NewsArticle(NewsArticleCreationDTO newsArticleCreationDTO)
        {
            NewsArticle newsArticle = new NewsArticle(Guid.NewGuid(),
                                   newsArticleCreationDTO.Title,
                                   newsArticleCreationDTO.Description,
                                   newsArticleCreationDTO.PublishDate,
                                   newsArticleCreationDTO.BuildingId,
                                   newsArticleCreationDTO.NewsTagTypeCreationDTO);
            if (newsArticleCreationDTO.Thumbnail is not null)
            {
                newsArticle.Thumbnail = newsArticleCreationDTO.Thumbnail;
            }
            if (newsArticleCreationDTO.TextContent is not null)
            {
                newsArticle.TextContent = newsArticleCreationDTO.TextContent;
            }
            if (newsArticleCreationDTO.MultiMediaContentDTOs is not null && newsArticleCreationDTO.MultiMediaContentDTOs.Count() > 0)
            {
                foreach (var item in newsArticleCreationDTO.MultiMediaContentDTOs)
                {
                    newsArticle.AddMultiMediaContent(item);     
                }
            }

            return newsArticle;
        }

    }
}
