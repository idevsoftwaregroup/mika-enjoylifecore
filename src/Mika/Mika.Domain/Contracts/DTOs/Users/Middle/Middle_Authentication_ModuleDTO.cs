using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mika.Domain.Contracts.DTOs.Users.Middle
{
    public class Middle_Authentication_ModuleDTO
    {
        public string Module { get; set; }
        public string Title { get; set; }
        public List<Middle_Authentication_SubModuleDTO> SubModules { get; set; }
    }

}
