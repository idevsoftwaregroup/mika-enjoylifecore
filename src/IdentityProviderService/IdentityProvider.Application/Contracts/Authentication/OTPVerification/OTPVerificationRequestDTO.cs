using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityProvider.Application.Contracts.Authentication.OTPVerification
{
    public class OTPVerificationRequestDTO
    {
        public string EmailOrPhoneNumber { get; set; }
        public string OTPValue { get; set; }
    }
}
