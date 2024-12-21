using news.domain.Models.Aggregates.NewsArticle.ValueObjects.Enums;

namespace news.application.Contracts.DTOs.NewsArticleDTOs
{
    public class NewsTagTypesDTO
    {
        public List<string> Values { get; set; } = new List<string>();

        public static explicit operator NewsTagTypesDTO(NewsTagType newsTagType)
        {
            NewsTagTypesDTO newsTagTypesDTO = new NewsTagTypesDTO();

            if (newsTagType.HasFlag(NewsTagType.FINANCIAL))
            {
                newsTagTypesDTO.Values.Add(nameof(NewsTagType.FINANCIAL));
            }
            if (newsTagType.HasFlag(NewsTagType.IMPORTANT))
            {
                newsTagTypesDTO.Values.Add(nameof(NewsTagType.IMPORTANT));
            }
            if (newsTagType.HasFlag(NewsTagType.REPAIREMENT))
            {
                newsTagTypesDTO.Values.Add(nameof(NewsTagType.REPAIREMENT));
            }
            
            return newsTagTypesDTO;
        }
        public static implicit operator NewsTagType(NewsTagTypesDTO dto)
        {
            NewsTagType result = NewsTagType.NONE; 

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