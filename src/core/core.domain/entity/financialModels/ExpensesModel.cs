using core.domain.entity.financialModels.valueObjects;
using core.domain.entity.structureModels;
using System.ComponentModel.DataAnnotations.Schema;

namespace core.domain.entity.financialModels;

public class ExpensesModel
{
    public long Id { get; set; }
    public string? Title { get; set; }
    public decimal Amount { get; set; }
    public string? RegisterNO { get; set; }
    public string? Description {  get; set; }
    public DateTime IssueDate { get; set; }
    public DateTime? DueDate { get; set; }
    public ExpenseType Type { get; set; }

    public UserModel? User { get; set; }

    public int? UnitModelId { get; set; }
    [ForeignKey("UnitModelId")]
    public virtual UnitModel Unit { get; set; }
    public bool? IsPaid { get; set; }
    public List<PaymentModel> Payments { get; set; }
    public DateTime? CreateDate { get; set; }=DateTime.Now;
}
