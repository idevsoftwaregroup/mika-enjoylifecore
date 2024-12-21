using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.domain.DomainModelDTOs.UserDTOs
{
    public class Response_CreatedUserDomainDTO
    {
        public int UserId { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
