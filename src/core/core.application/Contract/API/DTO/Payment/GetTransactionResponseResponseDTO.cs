using core.application.Contract.API.DTO.Expense;
using core.domain.entity.financialModels;
using core.domain.entity.financialModels.valueObjects;

namespace core.application.Contract.API.DTO.Payment;

public class GetTransactionResponseResponseDTO
{
    public int Id { get; set; }
    public string? ExtraDetail { get; set; }
    public bool IsSuccess { get; set; }
    public string? RefID { get; set; }
    public int? Status { get; set; }
    public DateTime CreatedDate { get; set; }
}
