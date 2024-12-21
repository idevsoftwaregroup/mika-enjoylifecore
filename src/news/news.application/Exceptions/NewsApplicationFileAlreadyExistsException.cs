namespace news.application.Exceptions
{
    public class NewsApplicationFileAlreadyExistsException : NewsApplicationException
    {
        public NewsApplicationFileAlreadyExistsException(string message = "unexpcted file already exists exception happened", Exception? innerException = null) : base(message, innerException)
        {
        }

    }
}
