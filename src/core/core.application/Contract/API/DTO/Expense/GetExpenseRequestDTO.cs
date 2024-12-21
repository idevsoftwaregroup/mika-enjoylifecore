using core.domain.entity.financialModels.valueObjects;

namespace core.application.Contract.API.DTO.Expense;

public class GetExpenseRequestDTO
{
    public int? UserId { get; set; }
    public int? UnitId { get; set; }
    public ExpenseType? ExpenseType { get; set; }
}

public class GetExpenseByUnitsRequestDTO
{
    public int? UserId { get; set; }
    public List<int>? UnitIds { get; set; }
    public ExpenseType? ExpenseType { get; set; }
}

public class GetExpenseFilterRequestDTO
{
    public int? UserId { get; set; }
    public int? UnitId { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public PaymentStateType? PaymentStatus { get; set; }
    public ExpenseType? ExpenseType { get; set; }
}
