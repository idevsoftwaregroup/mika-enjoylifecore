using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.application.Contract.API.DTO.Reservation.Reservation
{
    public class Response_GetYearMonthDTO
    {
        public int Year { get; set; }
        public List<Middle_GetYearMonth_MonthDTO> Months { get; set; }

    }
    public class Middle_GetYearMonth_MonthDTO
    {
        public int Month { get; set; }
        public string MonthTitle { get; set; }
    }
}
