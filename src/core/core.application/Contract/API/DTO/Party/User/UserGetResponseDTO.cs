using core.domain.entity.enums;
using System.ComponentModel.DataAnnotations;

namespace core.application.Contract.API.DTO.Party.User;

public class UserGetResponseDTO
{
    public int Id { get; set; }
    [Required]
    public string FirstName { get; set; }
    [Required]
    public string LastName { get; set; }
    public string? Email { get; set; }
    [Required]
    public string PhoneNumber { get; set; }
    public string? NationalID { get; set; }
    public string? Address { get; set; }
    public int? Age { get; set; }
    public GenderType? Gender { get; set; }
    
}
