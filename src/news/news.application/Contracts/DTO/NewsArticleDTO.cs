
using news.application.Utilities;
using news.domain.Models;

namespace news.application.Contracts.DTO
{
    public class NewsArticleDTO
    {
        public Guid Id { get; set; }
        public Guid BuildingId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string TextContent { get; set; }
        public string PublishDate { get; set; }
        public string PersianPublishDate { get; set; }
        public MultiMedia? Thumnail { get; set; }
        public HashSet<MultiMedia> MultiMedias { get; set; }
        public bool Important { get; set; } = false;
        public NewsTagType NewsTag { get; set; } = NewsTagType.ETC;
        //public NewsTagTypesDTO NewsTags { get; set; }
        public bool Pinned { get; set; } = false;

        public HashSet<Viewer> Viewers { get; set; }

        public static implicit operator NewsArticleDTO(NewsArticle newsArticle)
        {
            return new NewsArticleDTO()
            {
                Id = newsArticle.Id,
                BuildingId = newsArticle.BuildingId,
                Title = newsArticle.Title,
                Description = newsArticle.Description,
                TextContent = newsArticle.TextContent,
                PublishDate = newsArticle.PublishDate.ConvertGregToJalaiMonthName(),
                Thumnail = newsArticle.Thumbnail,
                MultiMedias = newsArticle.MultiMedias,
                NewsTag = newsArticle.NewsTag,
                Important = newsArticle.Important,
                Pinned = newsArticle.Pinned,
                Viewers = newsArticle.Viewers,
                PersianPublishDate = newsArticle.PublishDate.ConvertGregToJalaiMonthName()
            };
        }
    }
}
