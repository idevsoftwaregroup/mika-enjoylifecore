namespace Mika.Api.Helpers.Models
{
    public class TokenModel
    {
        public string Name { get; set; }
        public string Token { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}
