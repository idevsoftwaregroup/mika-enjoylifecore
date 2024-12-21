using Mika.Application.Services.Interfaces;
using Mika.Domain.Contracts.DTOs.BiDataEntry;
using Mika.Domain.Contracts.DTOs.BiDataEntry.Filter;
using Mika.Framework.Models;
using Mika.Framework.Models.Pagination;
using Mika.Infastructure.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mika.Application.Services.Implementations
{
    public class BiApplication : IBiApplication
    {
        private readonly IBiRepository _biRepository;
        public BiApplication(IBiRepository biRepository)
        {
            this._biRepository = biRepository;
        }
        public async Task<OperationResult<object>> CreateBiDataEntry(long CreatorId, Request_CreateBiDataEntryDTO model, CancellationToken cancellationToken = default)
        {
            return await _biRepository.CreateBiDataEntry(CreatorId, model, cancellationToken);
        }

        public async Task<OperationResult<PagedList<Response_GetBiDataEntryDTO>>> GetBiDataEntries(long UserId, Filter_GetBiDataEntryDTO? filter, PageModel? page, CancellationToken cancellationToken = default)
        {
            return await _biRepository.GetBiDataEntries(UserId, filter, page, cancellationToken);
        }
    }
}
