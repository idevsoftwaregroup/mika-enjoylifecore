namespace news.infrastructure.Exceptions
{
    public class NewsInfrastructurePersistenceConstraintException : NewsInfrastructureException
    {
        public NewsInfrastructurePersistenceConstraintException(string? message = "unexpeted infrastructure persistence constraint error happened", Exception? innerException = null) : base(message, innerException)
        {
        }
    }
}
