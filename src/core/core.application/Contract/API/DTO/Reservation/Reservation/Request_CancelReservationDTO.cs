using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.application.Contract.API.DTO.Reservation.Reservation
{
    public class Request_CancelReservationDTO
    {
        public long ReservationId { get; set; }
        public string? Description { get; set; }
    }
}
