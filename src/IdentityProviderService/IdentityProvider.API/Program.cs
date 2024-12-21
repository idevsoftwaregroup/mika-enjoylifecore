using IdentityProvider.API.DependencyInjection;
using IdentityProvider.Application.Interfaces;
using IdentityProvider.Application.Interfaces.Infrastructure;
using IdentityProvider.Application.Services;
using IdentityProvider.Domain.Models.EnjoyLifeRole;
using IdentityProvider.Domain.Models.EnjoyLifeUser;
using IdentityProvider.Infrastructure.Framework.Security.Implementations;
using IdentityProvider.Infrastructure.Framework.Security.Services;
using IdentityProvider.Infrastructure.Services.Authentication;
using IdentityProvider.Infrastructure.Services.Messaging;
using IdentityProvider.Infrastructure.Services.Persistence;
using IdentityProvider.Infrastructure.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Net.Http.Headers;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
// Add services to the container.

builder.Services.AddControllers();

builder.Services.Configure<JWTSettings>(builder.Configuration.GetSection(JWTSettings.SECTION_NAME));
builder.Services.Configure<OTPSettings>(builder.Configuration.GetSection(OTPSettings.SECTION_NAME));

builder.Services.AddScoped<IAuthenticationService,AuthenticationService>();
builder.Services.AddScoped<IOTPService,OTPService>();
builder.Services.AddScoped<IJWTTokenGenerator, JWTTokenGenerator>();
builder.Services.AddScoped<IMessagingService, MessagingService>();
builder.Services.AddScoped<IEnjoyLifeIdentityRepository, EnjoyLifeIdentityRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();


builder.Services.AddHttpClient<IMessagingHttpClient,MessagingHttpClient>()
//    (client => { 

//    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
    
//})
    //remove this , only for testing in development
    .ConfigurePrimaryHttpMessageHandler(() =>
    {
        var handler = new HttpClientHandler();
        handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
        return handler;
    });

builder.Services.AddDbContext<EnjoyLifeIdentityDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddIdentity<EnjoyLifeUser, EnjoyLifeRole>(options =>
{
    options.Password.RequiredLength = 0;
    options.SignIn.RequireConfirmedPhoneNumber = false;
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    
})
    .AddRoles<EnjoyLifeRole>()
    .AddEntityFrameworkStores<EnjoyLifeIdentityDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddJWTAuthenticationServices(builder.Configuration);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(policyBuilder =>
    policyBuilder.AddDefaultPolicy(policy =>
        policy.WithOrigins("*").AllowAnyHeader().AllowAnyHeader())
);

var app = builder.Build();
app.UseCors();


if (app.Environment.IsDevelopment())
{

}


app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();
app.MapControllers();
app.Run();
