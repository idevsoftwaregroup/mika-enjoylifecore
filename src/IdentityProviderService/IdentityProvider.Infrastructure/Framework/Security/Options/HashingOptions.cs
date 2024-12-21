using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityProvider.Infrastructure.Framework.Security.Options
{
    public class HashingOptions
    {
        public int Iterations { get; set; } = 10000;
    }
}
