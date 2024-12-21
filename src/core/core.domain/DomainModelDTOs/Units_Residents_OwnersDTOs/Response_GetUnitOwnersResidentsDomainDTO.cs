using core.domain.DomainModelDTOs.UserDTOs;
using core.domain.entity.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.domain.DomainModelDTOs.Units_Residents_OwnersDTOs
{
    public class Response_GetUnitOwnersResidentsDomainDTO
    {
        public int TotalCount { get; set; }
        public List<GetUnitOwnersResidentsDomainDTO>? Persons { get; set; }
    }
    public class GetUnitOwnersResidentsDomainDTO
    {
        //owner or resident data
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? NationalID { get; set; }
        public string? Address { get; set; }
        public int? Age { get; set; }
        public GenderType? Gender { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public bool? IsHiddenPhoneNumber { get; set; }
        public bool IsOwner { get; set; }
        public bool? IsHead { get; set; }
        public bool? Renting { get; set; }

        //unit data
        public int UnitId { get; set; }
        public string UnitName { get; set; }
        public int Floor { get; set; }
        public int ComplexId { get; set; }
        public int Block { get; set; }
    }

}
