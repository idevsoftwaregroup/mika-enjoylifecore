using core.application.Contract.API.DTO.Concierge;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.application.Contract.API.Interfaces
{
    public interface IConciergeService
    {
        Task<List<ConciergeResponseDTO>> GetConcierge(CancellationToken cancellationToken = default);
    }
}
