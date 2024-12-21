using news.application.Exceptions;
using news.domain.Models;
using System.ComponentModel.DataAnnotations;

namespace news.application.Contracts.DTO
{
    public class NewsArticlePreviewQueryParams
    {
        public const int MAX_PAGE_SIZE = 50;

        private Guid buildingId;
        [Required]
        public Guid BuildingId { get => buildingId; set => buildingId = value == Guid.Empty ? throw new NewsApplicationException("must provide building id in query params") : value; }


        public NewsTagType? NewsTag { get; set; }
        public DateTime? FromDate { get; set; }
        public bool? Important { get; set; }


        //#region fromDate
        //private DateTime? _fromDate;
        //public DateTime? FromDate
        //{
        //    get => _fromDate;
        //    set
        //    {

        //        if (_toDate is not null && value is not null)
        //        {
        //            if (_toDate <= value) throw new ApplicationException("FromDate should be before ToDate");
        //            _fromDate = value;

        //        }
        //        else _fromDate = value;
        //    }
        //}
        //#endregion

        //#region toDate
        //private DateTime? _toDate;
        //public DateTime? ToDate
        //{
        //    get => _toDate;
        //    set
        //    {
        //        if (_fromDate is not null && value is not null)
        //        {
        //            if (value <= _fromDate) throw new ApplicationException("FromDate should be before ToDate");
        //            _toDate = value;

        //        }
        //        else _toDate = value;
        //    }
        //}
        //#endregion

        #region pagination
        private int _pageNumber = 1;
        private int _pageSize = 10;

        public int PageNumber { get => _pageNumber; set => _pageNumber =/*pageNumber = value<1?1:*/value; } // probably put custom validation so i can send 400 instead of 500

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > 50 ? MAX_PAGE_SIZE : value; // question: throw exception here ??
        }
        #endregion

    }
}
