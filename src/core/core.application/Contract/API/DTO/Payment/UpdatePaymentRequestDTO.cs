using core.domain.entity.financialModels.valueObjects;

namespace core.application.Contract.API.DTO.Payment;

public class UpdatePaymentRequestDTO
{
    public PaymentStateType PaymentState {  get; set; }
    public string? Description { get; set; }
}
