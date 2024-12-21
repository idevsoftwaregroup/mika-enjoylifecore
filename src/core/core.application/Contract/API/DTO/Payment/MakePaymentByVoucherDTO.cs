namespace core.application.Contract.API.DTO.Payment;

public class MakePaymentByVoucherDTO
{
    public string BankVoucherId { get; set; }
    public long PaymentId { get; set; }
    public string VoucherImage { get; set; }

}
