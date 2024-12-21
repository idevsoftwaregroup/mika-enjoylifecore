namespace news.application.Exceptions
{
    public class NewsApplicationResourceNotFoundException : NewsApplicationException
    {
        public NewsApplicationResourceNotFoundException(string message = "unexpected requested resource was not found", Exception? innerException = null) : base(message, innerException)
        {
        }
    }
}
