using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityProvider.Application.Contracts.Authentication.BackDoor
{
    public class BackDoorDTO
    {
        public string Phonenumber { get; set; }
        public string OTP { get; set; }
    }
}
