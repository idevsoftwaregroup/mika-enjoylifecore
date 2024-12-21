using core.api;
using core.api.Services;
using core.api.Settings;
using core.application.contract.api.DTO.Payment;
using core.application.Contract.API.Interfaces;
using core.application.Contract.infrastructure;
using core.application.Contract.infrastructure.Services;
using core.application.Contract.Infrastructure;
using core.application.Services;
using core.infrastructure.Data.persist;
using core.infrastructure.Data.repository;
using core.infrastructure.FileServices;
using core.infrastructure.IdentityService;
using core.infrastructure.MessagingService;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using IdentityProvider.Infrastructure.Framework.Security.Implementations;
using IdentityProvider.Infrastructure.Framework.Security.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

var options = new JsonSerializerOptions
{
    Converters = { new PersianDateTimeConverter() },
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
};
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new PersianDateTimeConverter());
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});

builder.Services.AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        });

builder.Services.Configure<FileStorageSettings>(builder.Configuration.GetSection(FileStorageSettings.SECTION_NAME));
builder.Services.Configure<MessagingSettings>(builder.Configuration.GetSection(MessagingSettings.SECTION_NAME));


builder.Services.Configure<JWTSettings>(builder.Configuration.GetSection(JWTSettings.SECTION_NAME));

builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();

builder.Services.AddScoped<IComplexRepository, ComplexRepository>();
builder.Services.AddScoped<IComplexService, ComplexService>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IUnitRepository, UnitRepository>();
builder.Services.AddScoped<IUnitService, UnitService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IOwnerRepository, OwnerRepository>();
builder.Services.AddScoped<IOwnerService, OwnerService>();
builder.Services.AddScoped<IResidentRepository, ResidentRepository>();
builder.Services.AddScoped<IResidentService, ResidentService>();
builder.Services.AddScoped<IManagerRepository, ManagerRepository>();
builder.Services.AddScoped<IManagerService, ManagerService>();
builder.Services.AddScoped<IExpenseService, ExpenseService>();
builder.Services.AddScoped<IExpenseRepository, ExpenseRepository>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<IViewRenderService, ViewRenderService>();
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<IReservationService, ReservationService>();
builder.Services.AddScoped<IReservationRepository, ReservationRepository>();
builder.Services.AddScoped<IActionRepository, ActionRepository>();
builder.Services.AddScoped<IActionService, ActionService>();
builder.Services.AddScoped<IActivityRepository, ActivityRepository>();
builder.Services.AddScoped<IActivityService, ActivityService>();
builder.Services.AddScoped<IConciergeRepository, ConciergeRepository>();
builder.Services.AddScoped<IConciergeService, ConciergeService>();
builder.Services.AddScoped<ILobbyAttendantRepostory, LobbyAttendantRepostory>();
builder.Services.AddScoped<ILobbyAttendantService, LobbyAttendantService>();
builder.Services.AddScoped<IWarehouseRepository, WarehouseRepository>();
builder.Services.AddScoped<IWarehouseService, WarehouseService>();
builder.Services.AddScoped<IBillRepository, BillRepository>();
builder.Services.AddScoped<IBillService, BillService>();
// Marketing Service Scope
builder.Services.AddScoped<IMarketingRepository, MarketingRepository>();
builder.Services.AddScoped<IMarketingService, MarketingService>();




builder.Services.AddRazorPages();
builder.Services.Configure<PaymentZarinGetURL>(builder.Configuration.GetSection("PaymentZarinGetURL"));

builder.Services.AddHttpClient<IFileStorageService, FileStorageService>().ConfigurePrimaryHttpMessageHandler(() =>
{
    var handler = new HttpClientHandler();
    handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
    return handler;
});
builder.Services.AddHttpClient<IMessagingService, MessagingService>().ConfigurePrimaryHttpMessageHandler(() =>
{
    var handler = new HttpClientHandler();
    handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
    return handler;
});
builder.Services.AddHttpClient<IIdentityService, IdentityService>().ConfigurePrimaryHttpMessageHandler(() =>
{
    var handler = new HttpClientHandler();
    handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
    return handler;
});


builder.Services.AddScoped<ITicketingService, TicketingService>();

builder.Services.AddScoped<ITicketingRepository, TicketingRepository>();

builder.Services.AddScoped<IRepository, Repository>();

builder.Services.AddDbContext<EnjoyLifeContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("default")));



builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "EnjoyLife Core API",
        Version = "v1",
        Contact = new OpenApiContact
        {
            Name = "Mika Partners",
            Url = new Uri("https://mikapartners.co/")
        }
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\""
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                { Type = ReferenceType.SecurityScheme, Id = "Bearer"}
            }
        , Array.Empty<string>()
        }
    });
});

//builder.Services.AddSingleton<CustomAuthorizeAttribute>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{


    JWTSettings settings = builder.Configuration.GetRequiredSection(JWTSettings.SECTION_NAME).Get<JWTSettings>();

    var secretkey = Encoding.UTF8.GetBytes(settings.Secret);
    var encryptionkey = Encoding.UTF8.GetBytes(settings.EncryptionKey);

    var validationParameters = new TokenValidationParameters
    {
        ClockSkew = TimeSpan.FromMinutes(settings.ExpiryMinutes), // default: 5 min
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

builder.Services.AddSignalR();





//builder.Services.AddCors(policyBuilder =>
//    policyBuilder.AddDefaultPolicy(policy =>
//        policy.WithOrigins("*").AllowAnyHeader().AllowAnyHeader())
//);
#region CORS

//builder.Services.AddCors(policyBuilder =>
//    policyBuilder.AddDefaultPolicy(policy =>
//        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod())
//);

builder.Services.AddCors(option =>
{
    option.AddPolicy("CorsPolicy", builder =>
    {
        builder.AllowAnyMethod()
               .AllowAnyHeader()
               .WithOrigins("http://localhost:3000", "http://localhost:3001", "https://localhost:3001", "https://localhost:3000", "https://app.enjoylife.ir", "https://panel.enjoylife.ir", "http://app.enjoylife.ir", "http://panel.enjoylife.ir", "http://localhost:3011", "http://172.18.100.158:8081")
               .AllowCredentials();
    });
});
#endregion

//builder.Services.AddControllers(options =>
//{
//    options.Filters.Add<CustomAuthorizeAttribute>();
//    //options.Filters.Add(new AuthorizeFilter("AdminPolicy"));
//});



//builder.Services.AddAuthorization(options =>
//{
//    options.AddPolicy("AdminPolicy", policy => policy.RequireRole("ADMIN"));
//   // options.AddPolicy("ManageStore", policy => policy.RequireClaim("Action", "ManageStore"));
//});

var app = builder.Build();
//app.UseCors();
app.UseCors("CorsPolicy");

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Enj API");
        options.EnableTryItOutByDefault();
    });
}
else
{
    ////app.UseExceptionHandler("/error-development");
    ////app.UseMiddleware<ExceptionMiddleware>();
}
app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();
//app.UseAuthorization();
//app.UseMiddleware<DateTimeFormatMiddleware>();
app.MapControllers();




app.MapHub<HubHelper>("/hub");
app.Run();

