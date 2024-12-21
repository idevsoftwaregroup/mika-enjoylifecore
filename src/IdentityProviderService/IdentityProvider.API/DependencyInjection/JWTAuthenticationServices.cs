using IdentityProvider.Infrastructure.Services.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace IdentityProvider.API.DependencyInjection
{
    public static class JWTAuthenticationServices
    {
        public static IServiceCollection AddJWTAuthenticationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(options => {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                

                JWTSettings settings = configuration.GetRequiredSection(JWTSettings.SECTION_NAME).Get<JWTSettings>();

                var secretkey = Encoding.UTF8.GetBytes(settings.Secret);
                var encryptionkey = Encoding.UTF8.GetBytes(settings.EncryptionKey);

                var validationParameters = new TokenValidationParameters
                {
                    ClockSkew = TimeSpan.FromMinutes(120), // default: 5 min
                    RequireSignedTokens = true,

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(secretkey),

                    RequireExpirationTime = true,
                    ValidateLifetime = true,

                    ValidateAudience = true, //default : false
                    ValidAudience = settings.Audience,

                    ValidateIssuer = true, //default : false
                    ValidIssuer = settings.Issuer,

                    TokenDecryptionKey = new SymmetricSecurityKey(encryptionkey)
                };

                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = validationParameters;
                
            });

            return services;
        }
    }
}
