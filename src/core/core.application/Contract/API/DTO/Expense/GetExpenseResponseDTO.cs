using core.domain.entity.financialModels.valueObjects;

namespace core.application.Contract.API.DTO.Expense;

public class GetExpenseResponseDTO
{
    public long Id { get; set; }
    public string Title { get; set; }
    public decimal Amount { get; set; }
    public string RegisterNO { get; set; }
    public string Description { get; set; }
    public string IssueDateTime { get; set; }
    public string DueDate { get; set; }
    public int? UserID { get; set; }
    public int? UnitId { get; set; }
    public string UnitName { get; set; }
    public ExpenseType Type { get; set; }

    //public string PersianIssueDateTime { get; set; }
    //public string PersianDueDateTime { get; set; }
}
