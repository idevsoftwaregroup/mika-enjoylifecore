using core.domain.entity.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.domain.DomainModelDTOs.UserDTOs
{
    public class Request_CreateResidentsOwnersOfUnitDomainDTO
    {
        public int UnitId { get; set; }
        public List<OwnersOfUnitDomainDTO>? Owners { get; set; }
        public List<ResidentsOfUnitDomainDTO>? Residents { get; set; }
    }
    public class OwnersOfUnitDomainDTO
    {
        public string Name { get; set; }
        public string Lastname { get; set; }
        public string PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? NationalID { get; set; }
        public string? Address { get; set; }
        public int? Age { get; set; }
        public GenderType? Gender { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        
    }
    public class ResidentsOfUnitDomainDTO
    {
        public string Name { get; set; }
        public string Lastname { get; set; }
        public string PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? NationalID { get; set; }
        public string? Address { get; set; }
        public int? Age { get; set; }
        public GenderType? Gender { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public bool IsHead { get; set; }
        public bool Renting { get; set; }
    }
}
