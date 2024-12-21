using IdentityProvider.Application.Interfaces.Infrastructure;
using IdentityProvider.Domain.Models.EnjoyLifeUser;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Xml.Linq;

namespace IdentityProvider.Infrastructure.Services.Authentication;

public class JWTTokenGenerator : IJWTTokenGenerator
{

    private readonly JWTSettings _settings;
    private readonly IEnjoyLifeIdentityRepository _identityRepository;

    public JWTTokenGenerator(IOptions<JWTSettings> settings, IEnjoyLifeIdentityRepository identityRepository)
    {
        _settings = settings.Value;
        _identityRepository = identityRepository;
    }

    public async Task<string> GenerateTokenAsync(EnjoyLifeUser user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Secret));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = (await _identityRepository.GetUserClaimsAsync(user)).ToList();
        claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
        claims.Add(new Claim("CoreUserId", user.CoreId.ToString()));

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            notBefore: DateTime.Now,
            expires: DateTime.Now.AddMinutes(_settings.ExpiryMinutes),            
            signingCredentials: credentials
        );

        var tokenHandler = new JwtSecurityTokenHandler();
        var jwt = tokenHandler.WriteToken(token);

        return jwt;
    }

    //public async Task<string> GenerateTokenAsync(EnjoyLifeUser user)
    //{
    //    var signingCredentials =
    //        new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Secret)), SecurityAlgorithms.HmacSha256);

    //    var encryptingCredentials =
    //        new EncryptingCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.EncryptionKey)), SecurityAlgorithms.Aes128KW, SecurityAlgorithms.Aes128CbcHmacSha256);

    //    List<Claim> claims = (await _identityRepository.GetUserClaimsAsync(user)).ToList() ;

    //    claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));

    //    var descriptor = new SecurityTokenDescriptor
    //    {
    //        Issuer = _settings.Issuer,
    //        Audience = _settings.Audience,
    //        IssuedAt = DateTime.Now,
    //        NotBefore = DateTime.Now.AddMinutes(_settings.NotBeforeMinutes),
    //        Expires = DateTime.Now.AddMinutes(_settings.ExpiryMinutes),
    //        SigningCredentials = signingCredentials,
    //        EncryptingCredentials = encryptingCredentials,
    //        Subject = new ClaimsIdentity(claims)
    //    };

    //    var tokenHandler = new JwtSecurityTokenHandler();

    //    var securityToken = tokenHandler.CreateToken(descriptor);

    //    var jwt = tokenHandler.WriteToken(securityToken);

    //    return jwt;

    //}
}
