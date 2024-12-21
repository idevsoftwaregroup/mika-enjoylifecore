using Mika.Domain.Contracts.DTOs.Modules.Middle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mika.Domain.Contracts.DTOs.Modules
{
    public class Response_GetModuleDTO
    {
        public int ModuleId { get; set; }
        public string ModuleName { get; set; }
        public string DisplayName { get; set; }
        public List<Middle_SubModuleDTO> SubModules { get; set; }
    }
}
