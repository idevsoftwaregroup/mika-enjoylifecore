using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace news.infrastructure.Core
{
    public class CoreSettings
    {
        public const string SECTION_NAME = nameof(CoreSettings);
        public string BaseURL { get; set; }
    }
}
