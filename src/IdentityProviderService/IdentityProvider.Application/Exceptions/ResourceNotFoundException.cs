namespace IdentityProvider.Application.Exceptions;

public class ResourceNotFoundException : IdentityProviderApplicationException
{
    public ResourceNotFoundException(string? message = "Resource Not Found", Exception? innerException = null) : base(message, innerException)
    {
    }
}
