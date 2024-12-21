using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.application.contract.api.DTO.Payment
{
    public class PaymentZarinGetURL
    {
        public string MerchantID { get; set; }
        public string CallbackURL { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string Currency { get; set; } = "IRR";

    }
}
