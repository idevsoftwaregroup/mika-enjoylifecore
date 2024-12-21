using core.domain.entity.financialModels.valueObjects;

namespace core.application.Contract.API.DTO.Expense;

public class CreateExpenseDTO
{
    public string? Title { get; set; }
    public decimal Amount { get; set; }
    public int UserId { get; set; }
    public int? UnitID { get; set; }
    public ExpenseType ExpenseType { get; set; }
    public string RegisterNO { get; set; }
    public DateTime? DueDate { get; set; }
    public string Description { get; set; }
}
