using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.application.contract.api.DTO.Payment
{
    public class PaymentZarinVerificationDTO
    {
        public decimal Amount { private set; get; }
        public String MerchantID { private set; get; }
        public String Authority { private set; get; }


        public PaymentZarinVerificationDTO(String MerchantID, decimal Amount, String Authority)
        {
            this.Amount = Amount;
            this.MerchantID = MerchantID;
            this.Authority = Authority;
        }
    }
}
