using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mika.Domain.Contracts.DTOs.Users.Middle
{
    public class Middle_CreateUser_ModuleDTO
    {
        public int ModuleId { get; set; }
        public List<int> SubModuleIDs { get; set; }
    }
}
