using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.application.Contract.API.DTO.Marketing
{
    public class Response_GetBookingById
    {
        [Key]
        public int Id { get; set; }
        public string UserProfileId { get; set; }
        public string ProjectId { get; set; }
        public int BookingBlock { get; set; }
        public int BookingFloor { get; set; }
        public string BookingUnits { get; set; }
        public string BookingTotalPrice { get; set; } // Total Price
        public string BookingPrepaidPrecentPrice { get; set; } // Precent of Booking Prepaid Price
        public string BookingInstallmentAccounting { get; set; } // Installment Accounting
        [AllowNull]
        public string? BookingDateUpdated { get; set; } = string.Empty;
    }
}
