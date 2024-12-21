using core.domain.entity.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.domain.DomainModelDTOs.UserDTOs
{
    public class Response_GetUserDomainDTO
    {
        public int TotalCount { get; set; }
        public List<Middle_GetUserDataDomainDTO>? Persons { get; set; }
    }
    public class Middle_GetUserDataDomainDTO
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsHiddenPhoneNumber { get; set; }
        public string RealPhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? NationalID { get; set; }
        public string? Address { get; set; }
        public int? Age { get; set; }
        public GenderType? Gender { get; set; }
        public List<Middle_GetResidencies_GetUserDataDomainDTO>? Residencies { get; set; }
        public List<Middle_GetOwnerships_GetUserDataDomainDTO>? Ownerships { get; set; }
    }
    public class Middle_GetResidencies_GetUserDataDomainDTO
    {
        public int UnitId { get; set; }
        public string UnitName { get; set; }
        public int Floor { get; set; }
        public int ComplexId { get; set; }
        public int Block { get; set; }
        public bool IsHead { get; set; }
        public bool Renting { get; set; }
    }
    public class Middle_GetOwnerships_GetUserDataDomainDTO
    {
        public int UnitId { get; set; }
        public string UnitName { get; set; }
        public int Floor { get; set; }
        public int ComplexId { get; set; }
        public int Block { get; set; }
    }
}
