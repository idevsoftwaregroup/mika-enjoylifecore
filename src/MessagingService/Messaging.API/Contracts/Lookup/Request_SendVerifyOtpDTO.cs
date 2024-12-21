namespace Messaging.API.Contracts.Lookup
{
    public class Request_SendVerifyOtpDTO
    {
        public string Phonenumber { get; set; }
        public string OTP { get; set; }
    }
}
