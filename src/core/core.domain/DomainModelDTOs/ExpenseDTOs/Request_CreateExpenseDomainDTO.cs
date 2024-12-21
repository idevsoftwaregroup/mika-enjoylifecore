using core.domain.entity.financialModels.valueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.domain.DomainModelDTOs.ExpenseDTOs
{
    public class Request_CreateExpenseDomainDTO
    {
        public int Row { get; set; }
        public string? Title { get; set; }
        public decimal Amount { get; set; }
        public string? RegisterNO { get; set; }
        public string? Description { get; set; }
        public DateTime? DueDate { get; set; }
        public ExpenseType Type { get; set; }
        public string UnitName { get; set; }
        public bool? IsPaid { get; set; }
    }
}
