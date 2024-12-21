namespace news.application.Exceptions
{
    public class NewsApplicationInvalidContentTypeException : NewsApplicationException
    {
        public NewsApplicationInvalidContentTypeException(string message = "unexpcted invalid content type", Exception? innerException = null) : base(message, innerException)
        {
        }
    }
}
