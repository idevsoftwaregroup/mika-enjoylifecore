using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.domain.DomainModelDTOs.ReservationDTOs
{
    public class Request_CreateJointReservationDomainDTO
    {
        public int SessionId { get; set; }
        public int MenCount { get; set; }
        public int WomenCount { get; set; }
        public int GuestMenCount { get; set; }
        public int GuestWomenCount { get; set; }
    }
}
