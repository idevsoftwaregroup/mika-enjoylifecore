namespace news.infrastructure.Exceptions
{
    public class NewsInfrastructureException : Exception
    {
        public NewsInfrastructureException(string? message = "unexpeted infrastructure error happened", Exception? innerException = null) : base(message, innerException)
        {
        }
    }
}
