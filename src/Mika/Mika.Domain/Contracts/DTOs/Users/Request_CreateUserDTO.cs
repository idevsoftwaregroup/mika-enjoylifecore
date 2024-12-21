using Mika.Domain.Contracts.DTOs.Users.Middle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mika.Domain.Contracts.DTOs.Users
{
    public class Request_CreateUserDTO
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Position { get; set; }
        public string? PersonnelNumber { get; set; }
        public string? Avatar { get; set; }
        public List<Middle_CreateUser_ModuleDTO> Modules { get; set; }
        public List<long>? AccountIDs { get; set; }
    }
}
