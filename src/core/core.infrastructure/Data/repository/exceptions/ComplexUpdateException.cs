namespace core.infrastructure.Data.repository.exceptions
{
    public class ComplexUpdateException : RepositoryException
    {
        public override string ErrorTitle { get; } = "Could not update the complex";
        public ComplexUpdateException(string? message) : base(message ?? "unexpected error while updating complex")
        {
        }

    }
}
