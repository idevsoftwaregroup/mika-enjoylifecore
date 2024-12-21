using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace core.domain.DomainModelDTOs.MIKAMarketingDTOs
{
    public class MIKAMarketingBooking
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
        public string BookingDateTimeRegistration { get; set; } // DateTime REgistration
        [AllowNull]
        public string? BookingDateUpdated { get; set; } = string.Empty;
    }
}
