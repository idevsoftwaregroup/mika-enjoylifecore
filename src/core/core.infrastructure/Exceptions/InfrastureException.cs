namespace core.infrastructure.Exceptions;

public class InfrastureException : Exception
{
    public InfrastureException(string errorDetail) : base($"Infrasture Expection - {errorDetail}")
    {
    }
}
