using core.application.Contract.API.DTO.Account;
using core.application.Contract.API.DTO.Complex;
using core.domain.entity.enums;
using core.domain.entity.financialModels;
using core.domain.entity.structureModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.application.Contract.API.Mapper
{
    public static class AccountMapper
    {
        public static AccountGetResponseDTO ConvertComplexModelToGetComplexResponseDTO(this AccountModel value)
        {
            return new AccountGetResponseDTO()
            {
                ComplexId=value.complex.Id,
                Name = value.Name,         
                Bank  =value.Bank,   
                AccountOwner = value.AccountOwner,
                MerchantID=value.MerchantID,         
                AccountNumber=value.AccountNumber,         
                CardNumber=value.CardNumber,         
                AccountType=value.AccountType,
                IBAN=value.IBAN,
            };
        }
    }
}
