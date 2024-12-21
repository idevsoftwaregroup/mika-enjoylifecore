using Mika.Domain.Contracts.DTOs.Users.Middle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mika.Domain.Contracts.DTOs.Users
{
    public class Response_AuthenticationDTO
    {
        public long UserId { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string? LastName { get; set; }
        public string FullName { get; set; }
        public string ShortName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Position { get; set; }
        public string? PersonnelNumber { get; set; }
        public string? Avatar { get; set; }        
        public string RoleName { get; set; }
        public string RoleDisplayName { get; set; }
        public List<Middle_Authentication_ModuleDTO> Modules { get; set; }
        public string Token { get; set; }
        public DateTime TokenExpirationDate { get; set; }
    }
}
