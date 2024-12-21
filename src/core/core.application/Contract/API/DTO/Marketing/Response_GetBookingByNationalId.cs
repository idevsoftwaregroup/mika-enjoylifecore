using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.application.Contract.API.DTO.Marketing
{
    public class Response_GetBookingByNationalId
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Full Name")]
        public string FullName { get; set; }
        [Display(Name = "National ID")]
        public string NationalId { get; set; }
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
        [Display(Name = "Project Location")]
        public string BookingProjectLocation { get; set; }
        [Display(Name = "Project Unit")]
        public string BookingProjectUnit { get; set; }
        [Display(Name = "Project Block")]
        public string BookingProjectBlock { get; set; }
        [Display(Name = "Project Level")]
        public string BookingProjectLevel { get; set; }
        [Display(Name = "Project Floor")]
        public string BookingProjectFloor { get; set; }
        [Display(Name = "Project DateTime Delivery")]
        public string BookingDateTimeDelivery { get; set; }
        [Display(Name = "Project DateTime Registration of the Booking From")]
        public DateTime BookingDateTimeRegistration { get; set; }
    }
}
