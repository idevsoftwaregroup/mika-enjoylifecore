using core.application.Contract.infrastructure;
using core.domain.entity.Concierge;
using core.infrastructure.Data.persist;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.infrastructure.Data.repository
{
    public class ConciergeRepository : IConciergeRepository
    {
        private readonly EnjoyLifeContext _contextConcierge;

        public ConciergeRepository(EnjoyLifeContext contextConcierge)
        {
            _contextConcierge = contextConcierge;
        }

        public async Task<List<ConciergeModel>> GetConciergeAsync(CancellationToken cancellationToken)
        {
            return await _contextConcierge.Concierge.AsNoTracking().ToListAsync(cancellationToken);
        }

    }
}
