using core.domain.entity.financialModels.valueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.application.Contract.API.DTO.Payment
{
    public class GetAllPaymentsDTO
    {
        public int? Id { get; set; }
        public decimal? Amount { get; set; }
        public int? CreateBy { get; set; }
        public int? UnitId { get; set; }
        public PaymentStateType? PaymentStatus { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        
    }
}
