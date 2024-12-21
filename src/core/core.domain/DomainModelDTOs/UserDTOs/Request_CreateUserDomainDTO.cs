using core.domain.entity.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.domain.DomainModelDTOs.UserDTOs
{
    public class Request_CreateUserDomainDTO
    {
        public string Name { get; set; }
        public string Lastname { get; set; }
        public string PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? NationalID { get; set; }
        public string? Address { get; set; }
        public int? Age { get; set; }
        public GenderType? Gender { get; set; }
        public bool? IsHiddenPhoneNumber { get; set; }
        public string? Password { get; set; }
        public List<Middle_CreateUser_ResidentDomainDTO>? Residency { get; set; }
        public List<Middle_CreateUser_OwnerDomainDTO>? Ownership { get; set; }
    }
    public class Middle_CreateUser_ResidentDomainDTO
    {
        public int UnitId { get; set; }
        public bool IsHead { get; set; }
        public bool Renting { get; set; }
    }
    public class Middle_CreateUser_OwnerDomainDTO
    {
        public int UnitId { get; set; }
    }

}
