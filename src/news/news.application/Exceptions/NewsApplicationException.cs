namespace news.application.Exceptions
{
    public class NewsApplicationException : Exception
    {
        public NewsApplicationException(string message = "unexpcted application exception happened", Exception? innerException = null) : base(message, innerException)
        {
        }
    }
}
