namespace news.domain.Exceptions
{
    public class NewsDomainException : Exception
    {

        public NewsDomainException(string message = "unexpected error happened in domain", Exception? innerException = null) : base(message, innerException)
        {
        }
    }
}
