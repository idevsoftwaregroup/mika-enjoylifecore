using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.domain.entity.financialModels
{
    //pls dont create new table for each bank, instead add column here
    public class TransactionRequestModel
    {
        public int Id { get; set; }
        public String? Description { get; set; }
        public decimal Amount { get; set; }
        public string? Authority { get; set; }
        public String? Mobile { get; set; }
        public String? Email { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;        
    }
}
