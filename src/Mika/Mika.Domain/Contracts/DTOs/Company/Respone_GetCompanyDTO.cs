using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mika.Domain.Contracts.DTOs.Company
{
    public class Respone_GetCompanyDTO
    {
        public long CompanyId { get; set; }
        public string CompanyNumber { get; set; }
        public string CompanyCode { get; set; }
        public string Name { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? LastModificationDate { get; set; }
       

    }
}
