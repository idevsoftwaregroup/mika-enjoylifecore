using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.application.Contract.API.DTO.LobbyAttendant
{
    public class Response_LoginLobbyAttendantDTO
    {
        public int UserId { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public bool IsLobbyAttendant { get; set; }
    }
}
