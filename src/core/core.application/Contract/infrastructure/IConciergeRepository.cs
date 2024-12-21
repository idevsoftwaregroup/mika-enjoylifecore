using core.domain.entity.Concierge;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.application.Contract.infrastructure
{
    public interface IConciergeRepository
    {
        Task<List<ConciergeModel>> GetConciergeAsync(CancellationToken cancellationToken = default);
    }
}
