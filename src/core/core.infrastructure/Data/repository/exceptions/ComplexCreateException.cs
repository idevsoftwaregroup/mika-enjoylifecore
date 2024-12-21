namespace core.infrastructure.Data.repository.exceptions
{
    public class ComplexCreateException : RepositoryException
    {
        public override string ErrorTitle { get; } = "Could not create the complex";
        public ComplexCreateException(string? message) : base(message ?? "unexpected error while creating complex")
        {

        }
    }
}
