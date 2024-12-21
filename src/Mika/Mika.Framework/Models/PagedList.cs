using Mika.Framework.Models.Pagination;
using Mika.Framework.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mika.Framework.Models
{
    public class PagedList<T>
    {
        public long TotalItemsCount { get; private set; }
        public long? TotalPagesCount { get; private set; }
        public long? PageContain { get; private set; }
        public long? CurrentPageNumber { get; private set; }
        public List<T> Data { get; private set; }
        public PagedList(List<T> Data, long? TotalItemsCount, PageModel page)
        {
            this.Data = Data;
            this.TotalItemsCount = TotalItemsCount.IsNotNullAndZero() ? Convert.ToInt64(TotalItemsCount) : 0;
            if (page != null && page.Contain.IsNotNullAndZero() && TotalItemsCount > 0)
            {
                this.PageContain = page.Contain;
                this.TotalPagesCount =
                    TotalItemsCount % page.Contain == 0
                    ? Convert.ToInt64(TotalItemsCount / page.Contain)
                    : Convert.ToInt64(TotalItemsCount / page.Contain) + 1;
                this.CurrentPageNumber = page.Number.IsNotNullAndZero() ? page.Number : 1;
            }
        }
    }
}
