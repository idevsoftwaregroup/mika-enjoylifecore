using news.domain.Models.Aggregates.NewsArticle.MultiMedia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace news.application.Contracts.DTOs.NewsArticleDTOs
{
    public class MultiMediaContentDTO
    {
        public Guid Id { get; set; }
        public string URL { get; set; }
        public string FormatType { get; set; }
        public long Size { get; set; }

        //public MultiMediaContentDTO(Guid id, string uRL, string formatType, long size)
        //{
        //    Id = id;
        //    URL = uRL;
        //    FormatType = formatType;
        //    Size = size;
        //}
        public static implicit operator MultiMediaContent(MultiMediaContentDTO dto)
        {
            return new MultiMediaContent(dto.Id,dto.URL,dto.FormatType,dto.Size);
        }
        public static explicit operator MultiMediaContentDTO?(MultiMediaContent? multiMediaContent)
        {
            if (multiMediaContent == null) return null;

            return new MultiMediaContentDTO
            {
                Id = multiMediaContent.Id,
                URL = multiMediaContent.URL,
                FormatType = multiMediaContent.FormatType,
                Size = multiMediaContent.Size
            };

            //return new MultiMediaContentDTO
            //(
            //    id: multiMediaContent.Id,
            //    uRL: multiMediaContent.URL,
            //    formatType: multiMediaContent.FormatType,
            //    size: multiMediaContent.Size
            //);
        }

    }
}
