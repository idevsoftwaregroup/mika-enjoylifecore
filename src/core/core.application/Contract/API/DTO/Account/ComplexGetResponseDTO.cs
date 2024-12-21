using core.domain.entity.enums;
using core.domain.entity.structureModels;
using System.ComponentModel.DataAnnotations;

namespace core.application.Contract.API.DTO.Account;

public class AccountGetResponseDTO
{
    public int ComplexId { get; set; }
    public string? Name { get; set; }
    public string? AccountOwner { get; set; }
    public string? Bank { get; set; }
    public String? MerchantID { get; set; }
    public string? AccountNumber { get; set; }
    public string? IBAN { get; set; }
    public string? CardNumber { get; set; }
    public int? AccountType { get; set; }
    
}
