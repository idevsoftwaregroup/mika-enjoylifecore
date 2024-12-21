using core.application.Contract.API.DTO.Expense;
using core.domain.entity.financialModels.valueObjects;

namespace core.application.Contract.API.DTO.Payment;

public class GetPaymentsDTO
{
    public long Id { get; set; }
    public string Name { get; set; }
    public int TransactionStatus { get; set; }
    public string Status { get; set; }
    public string PayNumber { get; set; }
    public string PayDate { get; set; }
    public decimal TotalCost { get; set; }
    public string PayType { get; set; }
    public string? UnitName { get; set; }
    public string? Title { get; set; }

}
public class GetExcelPaymentsDTO
{
    public long Id { get; set; }
    public string PayBy { get; set; }
    public string Status { get; set; }
    public string PayNumber { get; set; }
    public string PayDate { get; set; }
    public decimal TotalCost { get; set; }
    public string PayType { get; set; }
    public string AccountName { get; set; }
    public string bankVoucherId { get; set; }
    public string bankReciveImagePath { get; set; }
}