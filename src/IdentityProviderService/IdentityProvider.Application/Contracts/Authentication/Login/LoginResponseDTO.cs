namespace IdentityProvider.Application.Contracts.Authentication.Login;

public record LoginResponseDTO
{
    public bool IsSuccessful { get; set; }
    public string? Error { get; set; } = null;
}
