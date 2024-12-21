using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.application.Contract.API.DTO.Reservation.Ticket
{
    public class GetPublicTicketsRequestDTO
    {
        public int Id { get; set; }
        public int unitId {  get; set; }        
        public DateTime? date { get; set; }
        public bool? withArchive { get; set; }=false;
    }
}
