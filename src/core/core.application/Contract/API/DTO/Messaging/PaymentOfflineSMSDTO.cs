namespace core.application.Contract.API.DTO.Messaging;

public class PaymentOfflineSMSDTO
{
    public string PaymenyId { get; set; }
    public string CreatedBy { get; set; }
    public string BankVoucherId { get; set; }
    public string BankVoucherImage { get; set; }
    public string AccountType { get; set; }
    public string TotalAmount { get; set; }
    public string CreatedDate { get; set; }
}
