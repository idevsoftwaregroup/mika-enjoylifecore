using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.application.Contract.API.DTO.Expense
{
    public class ExcelFileDTO
    {
        public IFormFile File { get; set; }
    }
}
