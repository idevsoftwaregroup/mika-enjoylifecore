using news.domain.Models;

namespace news.application.Contracts.DTO
{
    public class NewsArticleCreationDTO
    {
        public Guid BuildingId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string TextContent { get; set; }
        public DateTime PublishDate { get; set; }
        public NewsTagType NewsTag { get; set; }
        public bool Important { get; set; } = false;
        //public NewsTagTypesDTO NewsTags { get; set; }
        public bool Pinned { get; set; } = false;

        public static implicit operator NewsArticle(NewsArticleCreationDTO dto)
        {
            return new NewsArticle(Guid.NewGuid())
            {

                BuildingId = dto.BuildingId,
                Title = dto.Title,
                Description = dto.Description,
                TextContent = dto.TextContent,
                PublishDate = dto.PublishDate,
                NewsTag = dto.NewsTag,
                Important = dto.Important,
                Pinned = dto.Pinned,

            };
        }

        public static void ApplyUpdate(ref NewsArticle newsArticle, NewsArticleCreationDTO newsArticleDTO)
        {
            newsArticle.BuildingId = newsArticleDTO.BuildingId;
            newsArticle.Title = newsArticleDTO.Title;
            newsArticle.Description = newsArticleDTO.Description;
            newsArticle.TextContent = newsArticleDTO.TextContent;
            newsArticle.PublishDate = newsArticleDTO.PublishDate;
            newsArticle.NewsTag = newsArticleDTO.NewsTag;
            newsArticle.Important = newsArticleDTO.Important;
            newsArticle.Pinned = newsArticleDTO.Pinned;

        }
    }


}
