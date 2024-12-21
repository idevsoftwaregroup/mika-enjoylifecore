using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.application.contract.api.DTO.Payment
{
    public class PaymentZarinResponseDTO
    {
        public String Authority { set; get; }
        public int Status { set; get; }
        public String PaymentURL { set; get; }
    }
}
