namespace news.application.Exceptions
{
    public class NewsApplicationResourceAlreadyExistsException : NewsApplicationException
    {
        public NewsApplicationResourceAlreadyExistsException(string message = "unexpcted application exception happened", Exception? innerException = null) : base(message, innerException)
        {
        }

        public NewsApplicationResourceAlreadyExistsException(IDictionary<string, string> data, string message = "unexpcted application exception happened", Exception? innerException = null) : base(message, innerException)
        {
            foreach (var pair in data)
            {
                Data.Add(pair.Key, pair.Value);
            }
        }
    }
}
