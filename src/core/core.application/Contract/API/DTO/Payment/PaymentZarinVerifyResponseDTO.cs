using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.application.contract.api.DTO.Payment
{
    public class PaymentZarinVerifyResponseDTO
    {
        public bool IsSuccess { get { return Status == 100; } set { this.IsSuccess = value; } }
        public string CreatedBy { get; set; }
        public decimal totalAmount { get; set; }
        public long PaymentId { get; set; }
        public String RefID { get; set; }
        public int Status { get; set; }
        public ExtraDetail ExtraDetail { get; set; }
    }
    public class ExtraDetail
    {
        public Transaction Transaction;
    }


    public class Transaction
    {
        public String CardPanHash;
        public String CardPanMask;
    }
}
