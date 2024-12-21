using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.application.Contract.API.DTO.Reservation.Reservation
{
    public class Response_GetUnitsReservedCountsDTO
    {
        public int ReservedInYear { get; set; }
        public int ReservedInMonth { get; set; }
        public int ReservedInWeek { get; set; }
        public int ReservedInDay { get; set; }
    }
}
