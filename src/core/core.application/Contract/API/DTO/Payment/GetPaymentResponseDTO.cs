using core.application.Contract.API.DTO.Expense;
using core.domain.entity.financialModels.valueObjects;

namespace core.application.Contract.API.DTO.Payment;

public class GetPaymentResponseDTO
{
    public long Id { get; set; }
    public decimal totalAmount { get; set; }
    public List<GetExpenseResponseDTO> expenses { get; set; }
    public PaymentStateType state { get; set; }
    public string createDate { get; set; }
    public string paymentDate { get; set; }
    public string? paymentTime { get; set; }  
    public string? PaymentBy {  get; set; }
    public PaymentType paymentType { get; set; }
    public string bankVoucherId { get; set; }
    public string bankReciveImagePath { get; set; }
}
