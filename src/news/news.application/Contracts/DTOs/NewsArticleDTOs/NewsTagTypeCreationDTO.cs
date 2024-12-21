using news.domain.Models.Aggregates.NewsArticle.ValueObjects.Enums;

namespace news.application.Contracts.DTOs.NewsArticleDTOs
{
    public class NewsTagTypeCreationDTO
    {
        public IEnumerable<string> Values { get; set; }

        public static implicit operator NewsTagType(NewsTagTypeCreationDTO dto) {
            NewsTagType result = NewsTagType.NONE;

            if (dto.Values is null) return result;

            if (dto.Values.Contains($"{NewsTagType.FINANCIAL}")) 
            {
                result = result | NewsTagType.FINANCIAL;
            }
            if (dto.Values.Contains($"{NewsTagType.IMPORTANT}"))
            {
                result = result | NewsTagType.IMPORTANT;
            }
            if (dto.Values.Contains($"{NewsTagType.REPAIREMENT}"))
            {
                result = result | NewsTagType.REPAIREMENT;
            }

            return result;
        }
    }
}