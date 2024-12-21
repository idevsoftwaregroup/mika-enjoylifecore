namespace core.application.Contract.API.DTO.Payment;

public class MakePaymentFinalizeDTO
{
    public string bankVoucherId { get; set; }
    public long PaymentId { get; set; }
    public string BankResponse { get; set; }
}
