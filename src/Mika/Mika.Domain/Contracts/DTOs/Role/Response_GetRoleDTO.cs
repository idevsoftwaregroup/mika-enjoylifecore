using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mika.Domain.Contracts.DTOs.Role
{
    public class Response_GetRoleDTO
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public string DisplayName { get; set; }
    }
}
