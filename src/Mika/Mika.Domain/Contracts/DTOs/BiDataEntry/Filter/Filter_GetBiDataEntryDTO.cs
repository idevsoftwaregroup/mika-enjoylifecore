using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mika.Domain.Contracts.DTOs.BiDataEntry.Filter
{
    public class Filter_GetBiDataEntryDTO
    {
        public long? BiDataEntryId { get; set; }
        public string? Search { get; set; }
        public DateTime? EntryDateTime { get; set; }
        public long? CreatorId { get; set; }
        public int? RoleId { get; set; }
        public long? AccountId { get; set; }
        public long? CompanyId { get; set; }
        public DateTime? CreationDate { get; set; }
    }
}
