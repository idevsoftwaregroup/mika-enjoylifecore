namespace IdentityProvider.Application.Exceptions;

public class IdentityProviderApplicationException : Exception
{
    public IdentityProviderApplicationException(string? message = "Unexpected Exception", Exception? innerException = null) : base(message, innerException)
    {
    }
}
