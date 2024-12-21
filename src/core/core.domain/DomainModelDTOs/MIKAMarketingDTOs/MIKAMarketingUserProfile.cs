using System;
using System.ComponentModel.DataAnnotations;

namespace core.domain.DomainModelDTOs.MIKAMarketingDTOs
{
    public class MIKAMarketingUserProfile
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Name of User")]
        public string UserFullName { get; set; }


        [Display(Name = "User Date of Birth")]
        public string? UserDateOfBirth { get; set; }

        [EmailAddress]
        [Display(Name = "User E-mail")]
        public string UserEmail { get; set; }

        [Required]
        [Display(Name = "User Phone Number")]
        public string UserPhoneNumber { get; set; }

        [Display(Name = "User Job Title")]
        public string? UserJobTitle { get; set; }

        [Required]
        [Display(Name = "User National ID")]
        public string UserNationalId { get; set; }
    }
}
