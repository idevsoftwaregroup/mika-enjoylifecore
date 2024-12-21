using Mika.Api.Helpers.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Mika.Framework.Utilities;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Newtonsoft.Json;
using Mika.Domain.Contracts.DTOs.Users.Middle;
namespace Mika.Api.Helpers.Services
{
    public class TokenHelper
    {
        private readonly IConfiguration _config;
        public TokenHelper(IConfiguration config)
        {
            this._config = config;
        }
        public TokenModel GenerateToken(long UserId, string UserName, string? RoleName, List<Middle_Authentication_ModuleDTO>? Modules)
        {
            if (!UserName.IsNotNull())
            {
                UserName = "Anonymous";
            }
            var now = DateTime.UtcNow;
            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenKey = Encoding.UTF8.GetBytes(this._config["JwtKey"]);
            var expDate = now.AddMinutes(this._config["JwtExpiryMinutes"] != null ? Convert.ToDouble(this._config["JwtExpiryMinutes"]) : 1500);

            string accessModulesClaimValue =
                JsonConvert.SerializeObject(Modules != null && Modules.Where(x => !string.IsNullOrEmpty(x.Module) && !string.IsNullOrWhiteSpace(x.Module)).Any() ? Modules.Where(x => !string.IsNullOrEmpty(x.Module) && !string.IsNullOrWhiteSpace(x.Module)).Select(x => new Middle_Authentication_ModuleDTO()
                {
                    Module = x.Module.ToUpper(),
                    Title = x.Title,
                    SubModules = x.SubModules.Select(y => new Middle_Authentication_SubModuleDTO
                    {
                        SubModule = y.SubModule.ToUpper(),
                        Title = y.Title
                    }).ToList()
                }) : new List<Middle_Authentication_ModuleDTO>());
            string rolenameClaimValue = !string.IsNullOrEmpty(RoleName) && !string.IsNullOrWhiteSpace(RoleName) ? RoleName.ToUpper() : string.Empty;

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[] { new(ClaimTypes.Name, UserName), new("UserId", $"{UserId}"), new("Rolename", rolenameClaimValue), new("Modules", accessModulesClaimValue) }),
                Expires = expDate,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return new TokenModel { Name = UserName, Token = tokenHandler.WriteToken(token), ExpirationDate = expDate };
        }
    }
}
