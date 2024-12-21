namespace news.infrastructure.Exceptions
{
    public class NewsInfrastructureFileAlreadyExistsException : NewsInfrastructureException
    {
        public NewsInfrastructureFileAlreadyExistsException(string? message = "unexpeted file already exists error happened", Exception? innerException = null) : base(message, innerException)
        {
        }
    }
}
