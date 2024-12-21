using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.application.Contract.API.DTO.Marketing
{
    public class Request_CreateUserProfile
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
