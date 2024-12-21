using core.application.Contract.API.DTO.Expense;
using core.domain.entity.financialModels.valueObjects;

namespace core.application.Contract.API.DTO.Payment;

public class GetPaymentDetailResponseDTO
{
    public decimal totalAmount { get; set; }
    public List<GetExpenseResponseDTO> expenses { get; set; }
    public PaymentStateType state { get; set; }
    public string createDate { get; set; }
    public string paymentDate { get; set; }
    public PaymentType paymentType { get; set; }
    public string bankVoucherId { get; set; }
    public string PayDate { get; set; }
    public string PayTime { get; set; }
    public string PayBy {  get; set; }
     
}
