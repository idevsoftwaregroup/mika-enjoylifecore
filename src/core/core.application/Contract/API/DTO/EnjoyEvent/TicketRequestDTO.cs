using core.domain.entity.EnjoyEventModels;
using core.domain.entity.structureModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.application.Contract.API.DTO.EnjoyEvent
{
    public class TicketRequestDTO
    {
        public long? Id { get; set; } = 0;
        public int UnitId { get; set; }
        public int? UserId { get; set; } = 0;
        public long SessionId { get; set; }
        public int MaleNum { get; set; }
        public int FemaleNum { get; set; }
        public int GuestMaleNum { get; set; } = 0;
        public int GuestFemaleNum { get; set; } = 0;
        public DateTime? CreatedAt { get; set; } = DateTime.Now;
        public DateTime? LastModified { get; set; } = DateTime.Now;
    }
}
