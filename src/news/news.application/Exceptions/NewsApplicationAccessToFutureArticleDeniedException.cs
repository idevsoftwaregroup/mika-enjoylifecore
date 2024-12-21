namespace news.application.Exceptions;

public class NewsApplicationAccessToFutureArticleDeniedException : NewsApplicationException
{
    public NewsApplicationAccessToFutureArticleDeniedException(string message = "unexpcted Access denied exception happened", Exception? innerException = null) : base(message, innerException)
    {
    }
}
