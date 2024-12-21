namespace core.infrastructure.Data.repository.exceptions
{
    internal class UnitUpdateException : RepositoryException
    {
        public override string ErrorTitle { get; } = "Could not update the unit";
        public UnitUpdateException(string? message) : base(message ?? "unexpected error while updating unit")
        {
        }
    }
}
