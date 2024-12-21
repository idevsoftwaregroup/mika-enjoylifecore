using core.domain.entity.financialModels.valueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.domain.displayEntities.financialModels
{
    public class PaymentModelDisplayDTO
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
        public decimal totalAmount { get; set; }
        public List<GetExpenseResponseDisplayDTO> expenses { get; set; }
        public PaymentStateType state { get; set; }
        public string createDate { get; set; }
        public string paymentDate { get; set; }
        public string? paymentTime { get; set; }
        public string? PaymentBy { get; set; }
        public PaymentType paymentType { get; set; }
        public string bankVoucherId { get; set; }
        public string bankReciveImagePath { get; set; }
    }
    public class GetExpenseResponseDisplayDTO
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
    }
}
