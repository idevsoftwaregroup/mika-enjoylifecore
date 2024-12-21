using core.application.Contract.API.DTO.Expense;
using core.domain.entity.financialModels;
using core.domain.entity.financialModels.valueObjects;

namespace core.application.Contract.API.DTO.Payment;

public class GetTransactionRequestResponseDTO
{
    public int Id { get; set; }
    public String? Description { get; set; }
    public decimal Amount { get; set; }
    public string? Authority { get; set; }
    public String? Mobile { get; set; }
    public String? Email { get; set; }
    public DateTime CreatedDate { get; set; }  // its not used but probably should be string if its to be persian
    public GetTransactionResponseResponseDTO TransactionResponseDTO { get; set; }
}
