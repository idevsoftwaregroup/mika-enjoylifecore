namespace core.infrastructure.Data.repository.exceptions
{
    public class UnitCreateException : RepositoryException
    {
        public override string ErrorTitle { get; } = "Could not create the unit";
        public UnitCreateException(string? message) : base(message ?? "unexpected error while creating unit")
        {
        }
    }
}
