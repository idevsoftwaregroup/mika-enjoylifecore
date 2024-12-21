using core.domain.entity.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.application.Contract.API.DTO.Reservation.Joint
{
    public class Response_GetComplexDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Address { get; set; }
        public string? Description { get; set; }
        public DirectionType? Directions { get; set; }
        public DirectionType? Positions { get; set; }
        public ComplexUsageType? Usages { get; set; }

    }
}
