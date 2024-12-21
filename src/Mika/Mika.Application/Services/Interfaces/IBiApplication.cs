using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mika.Domain.Contracts.DTOs.BiDataEntry;
using Mika.Domain.Contracts.DTOs.BiDataEntry.Filter;
using Mika.Framework.Models;
using Mika.Framework.Models.Pagination;
namespace Mika.Application.Services.Interfaces
{
    public interface IBiApplication
    {
        Task<OperationResult<PagedList<Response_GetBiDataEntryDTO>>> GetBiDataEntries(long UserId, Filter_GetBiDataEntryDTO? filter, PageModel? page, CancellationToken cancellationToken = default);
        Task<OperationResult<object>> CreateBiDataEntry(long CreatorId, Request_CreateBiDataEntryDTO model, CancellationToken cancellationToken = default);
    }
}
