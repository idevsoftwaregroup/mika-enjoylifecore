using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mika.Domain.Contracts.DTOs.Users.Filter
{
    public class Filter_GetUserDTO
    {
        public long? UserId { get; set; }
        public bool? Active { get; set; }
        public string? Search { get; set; }
        public int? RoleId { get; set; }
        public DateTime? CreationDate { get; set; }

    }
}
