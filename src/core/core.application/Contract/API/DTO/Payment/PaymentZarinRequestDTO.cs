using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.application.contract.api.DTO.Payment
{
    public class PaymentZarinRequestDTO
    {
        public int buildingId { get; set; }
        public int RequestBy { get; set; }
        public int AccountType { get; set; }
        public String CallbackURL { get; set; }
        public String? Description { get; set; }
        public long PaymentId { get; set; }
       
    }
}
