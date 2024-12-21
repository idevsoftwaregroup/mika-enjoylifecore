namespace news.application.Contracts.DTO
{
    public class NewsTagTypesDTO
    {


        //public static explicit operator NewsTagTypesDTO(NewsTagType newsTagType)
        //{
        //    NewsTagTypesDTO newsTagTypesDTO = new NewsTagTypesDTO();

        //    if (newsTagType.HasFlag(NewsTagType.FINANCIAL))
        //    {
        //        newsTagTypesDTO.Values.Add(nameof(NewsTagType.FINANCIAL));
        //    }
        //    if (newsTagType.HasFlag(NewsTagType.IMPORTANT))
        //    {
        //        newsTagTypesDTO.Values.Add(nameof(NewsTagType.IMPORTANT));
        //    }
        //    if (newsTagType.HasFlag(NewsTagType.REPAIREMENT))
        //    {
        //        newsTagTypesDTO.Values.Add(nameof(NewsTagType.REPAIREMENT));
        //    }
        //    if (newsTagType.HasFlag(NewsTagType.MANAGMENT))
        //    {
        //        newsTagTypesDTO.Values.Add(nameof(NewsTagType.MANAGMENT));
        //    }

        //    return newsTagTypesDTO;
        //}
        //public static implicit operator NewsTagType(NewsTagTypesDTO dto)
        //{
        //    NewsTagType result = NewsTagType.NONE;

        //    if (dto.Values.Contains($"{NewsTagType.FINANCIAL}"))
        //    {
        //        result = result | NewsTagType.FINANCIAL;
        //    }
        //    if (dto.Values.Contains($"{NewsTagType.IMPORTANT}"))
        //    {
        //        result = result | NewsTagType.IMPORTANT;
        //    }
        //    if (dto.Values.Contains($"{NewsTagType.REPAIREMENT}"))
        //    {
        //        result = result | NewsTagType.REPAIREMENT;
        //    }
        //    if (dto.Values.Contains($"{NewsTagType.MANAGMENT}"))
        //    {
        //        result = result | NewsTagType.MANAGMENT;
        //    }

        //    return result;
        //}
    }
}