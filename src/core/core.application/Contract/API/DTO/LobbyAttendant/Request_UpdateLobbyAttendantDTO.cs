using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.application.Contract.API.DTO.LobbyAttendant
{
    public class Request_UpdateLobbyAttendantDTO
    {
        public int LobbyAttendantId { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public bool Active { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
