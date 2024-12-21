using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.application.Contract.infrastructure.Services
{
    public interface IIdentityService
    {
        Task<string> GetToken(int CoreId,CancellationToken cancellationToken = default);
    }
}
