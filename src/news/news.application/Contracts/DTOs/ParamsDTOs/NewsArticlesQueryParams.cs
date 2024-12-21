using news.application.Contracts.DTOs.NewsArticleDTOs;
using news.domain.Models.Aggregates.NewsArticle.ValueObjects.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace news.application.Contracts.DTOs.ParamsDTOs
{
    public class NewsArticlesQueryParams
    {
        private const int MAX_PAGE_SIZE = 50;

        public NewsTagTypesDTO? NewsTagTypeDTO { get; set; }
        public Guid? BuildingId { get; set; }

        #region fromDate
        private DateTime? _fromDate;
        public DateTime? FromDate { get => _fromDate;
            set
            {
                
                if (_toDate is not null && value is not null)
                {
                    if (_toDate <= value) throw new ApplicationException("FromDate should be before ToDate");
                    _fromDate = value;

                }else _fromDate = value;
            }
        }
        #endregion

        #region toDate
        private DateTime? _toDate;
        public DateTime? ToDate { get => _toDate; 
            set 
            {
                if (_fromDate is not null && value is not null)
                {
                    if (value <= _fromDate) throw new ApplicationException("FromDate should be before ToDate");
                    _toDate = value;

                }else _toDate = value;
            }
        }
        #endregion

        #region pagination
        public int PageNumber { get; set; } = 1;
        private int _pageSize = 10;
        public int PageSize {
            get => _pageSize;
            set => _pageSize = value>50 ? MAX_PAGE_SIZE:value ; // question: throw exception here ??
        }

        #endregion
        
        //public NewsArticlesQueryParams(DateTime? fromDate, DateTime? toDate)
        //{
        //    if (fromDate != null && toDate != null) //question: should I check this in infrastructure layer
        //    {
        //        if (fromDate >= ToDate) throw new ArgumentException("fromDate should be later than toDate");
        //    }

        //    FromDate = fromDate;
        //    ToDate = toDate;
        //}



    }
}
