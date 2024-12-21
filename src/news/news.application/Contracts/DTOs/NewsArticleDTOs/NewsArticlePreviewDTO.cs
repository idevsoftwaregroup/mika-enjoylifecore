using news.domain.Models.Aggregates.NewsArticle;
using news.domain.Models.Aggregates.NewsArticle.ValueObjects.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace news.application.Contracts.DTOs.NewsArticleDTOs
{
    public class NewsArticlePreviewDTO
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public DateTime PublishDate { get; set; }
        public Guid BuildingId { get; set; }
        public NewsTagTypesDTO? NewsTagTypes { get; set; }
        public MultiMediaContentDTO? ThumbnailMetaData { get; set; }
        public int NumberOfViews { get; set; }

        public static explicit operator NewsArticlePreviewDTO(NewsArticle newsArticle)
        {
            return new NewsArticlePreviewDTO
            {
                Id = newsArticle.Id,
                Title = newsArticle.Title,
                Description = newsArticle.Description,
                PublishDate = newsArticle.PublishDate,
                BuildingId = newsArticle.BuildingId,
                NewsTagTypes = (NewsTagTypesDTO)newsArticle.NewsTagTypes,
                ThumbnailMetaData = (MultiMediaContentDTO?)newsArticle.Thumbnail,
                NumberOfViews = newsArticle.Viewers.Count()
            };

        }

        
    }

}
