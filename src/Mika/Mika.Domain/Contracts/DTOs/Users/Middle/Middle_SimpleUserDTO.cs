using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mika.Domain.Contracts.DTOs.Users.Middle
{
    public class Middle_SimpleUserDTO
    {
        public long UserId { get; set; }
        public int RoleId { get; set; }
        public long? PrimaryParentId { get; set; }
        public List<long>? SecodaryParentIDs { get; set; }

    }
}
