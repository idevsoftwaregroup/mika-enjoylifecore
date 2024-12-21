using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.application.Contract.API.DTO.LobbyAttendant.Filter
{
    public class Filter_GetLobbyAttendantDTO
    {
        public string? Search { get; set; }
        public bool? Active { get; set; }
    }
}
