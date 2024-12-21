using System.Net;

namespace Messaging.API.Contracts.Lookup
{
    public class Response_SendVerifyOtpDTO
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public string? ExMessage { get; set; }
        public HttpStatusCode Status { get; set; }
    }
}
