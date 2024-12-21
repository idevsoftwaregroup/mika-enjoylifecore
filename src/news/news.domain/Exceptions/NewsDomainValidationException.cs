namespace news.domain.Exceptions
{
    public class NewsDomainValidationException : NewsDomainException
    {
        public NewsDomainValidationException(string message = "unexpected validation error happened", Exception? innerException = null) : base(message, innerException)
        {
        }
    }
}
