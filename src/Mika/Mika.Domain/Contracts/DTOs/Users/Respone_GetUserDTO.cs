using Mika.Domain.Contracts.DTOs.Account;
using Mika.Domain.Contracts.DTOs.Modules;
using Mika.Domain.Contracts.DTOs.Users.Middle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mika.Domain.Contracts.DTOs.Users
{
    public class Respone_GetUserDTO
    {
        public long UserId { get; set; }
        public string UserName { get; set; }
        public bool Active { get; set; }
        public string Name { get; set; }
        public string? LastName { get; set; }
        public string FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Position { get; set; }
        public string? PersonnelNumber { get; set; }
        public string? Avatar { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public string RoleDisplayName { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? LastModificationDate { get; set; }
        public Middle_ParentUserDTO? PrimaryParent { get; set; }
        public List<Middle_ParentUserDTO>? SecondaryParents { get; set; }
        public List<Response_GetModuleDTO> Modules { get; set; }
        public List<Response_GetAccountDTO>? Accounts { get; set; }
    }
}
