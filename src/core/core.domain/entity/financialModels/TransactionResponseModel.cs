using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 

namespace core.domain.entity.financialModels
{
    //pls dont create new table for each bank, instead add column here
    public class TransactionResponseModel
    {
        public int Id { get; set; }
        public string? ExtraDetail { get; set; }
        public bool IsSuccess { get; set; }
        public string? RefID { get; set; } 
        public int? Status{ get; set; }
        public DateTime CreatedDate { get; set; }= DateTime.Now;
    }
}
