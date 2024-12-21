using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.application.Contract.API.DTO.LobbyAttendant
{
    public class Request_LoginLobbyAttendantDTO
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
