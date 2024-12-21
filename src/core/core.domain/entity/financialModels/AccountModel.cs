using core.domain.entity.structureModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.domain.entity.financialModels
{
    public class AccountModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string AccountOwner { get; set; }
        public string Bank { get; set; }
        public String? MerchantID { get; set; }
        public string AccountNumber { get; set; }
        public string IBAN { get; set; }
        public string CardNumber { get; set; }
        public int AccountType { get; set; }
        public ComplexModel complex { get; set; }
    }
}
