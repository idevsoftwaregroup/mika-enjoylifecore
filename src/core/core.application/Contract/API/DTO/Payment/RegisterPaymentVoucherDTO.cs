namespace core.application.Contract.API.DTO.Payment;

public class RegisterPaymentVoucherDTO
{    
    public long Id { get; set; }
    public string bankVoucherId { get; set; } = string.Empty;
    public string bankReciveImagePath { get; set;} = string.Empty;
    public int ComplexId { get; set; }   
    public int AccountType { get; set; }
}
