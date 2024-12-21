namespace core.infrastructure.Data.repository.exceptions
{
    public class RepositoryException : Exception
    {
        public virtual string ErrorTitle { get; }
        public RepositoryException(string? message) : base(message)
        {
            ErrorTitle = "error in persisting data";
        }
    }
}
