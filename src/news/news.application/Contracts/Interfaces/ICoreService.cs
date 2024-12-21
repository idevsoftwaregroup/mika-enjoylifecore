using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace news.application.Contracts.Interfaces
{
    public interface ICoreService
    {
        Task<List<string>> GetPhoneNumbers(int userId, int complexId, CancellationToken cancellationToken = default);
    }
}
