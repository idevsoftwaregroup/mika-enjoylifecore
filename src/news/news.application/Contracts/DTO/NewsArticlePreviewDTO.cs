
using news.application.Utilities;
using news.domain.Models;

namespace news.application.Contracts.DTO
{
    public class NewsArticlePreviewDTO
    {
        public Guid Id { get; set; }
        public Guid BuildingId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime PublishDate { get; set; }
        public MultiMedia? Thumnail { get; set; }
        //public NewsTagTypesDTO NewsTags { get; set; }
        public NewsTagType NewsTag { get; set; }
        public bool Important { get; set; } = false;
        public bool Pinned { get; set; }
        public string PersianPublishDate { get; set; }

        public static explicit operator NewsArticlePreviewDTO(NewsArticle newsArticle)
        {
            return new NewsArticlePreviewDTO()
            {
                Id = newsArticle.Id,
                BuildingId = newsArticle.BuildingId,
                Title = newsArticle.Title,
                Description = newsArticle.Description,
                PublishDate = newsArticle.PublishDate,
                Thumnail = newsArticle.Thumbnail,
                //NewsTags = (NewsTagTypesDTO) newsArticle.NewsTag
                NewsTag = newsArticle.NewsTag,
                Important = newsArticle.Important,
                Pinned = newsArticle.Pinned,
                PersianPublishDate = newsArticle.PublishDate.ConvertGeoToJalaiSimple()
            };
        }
    }
}
