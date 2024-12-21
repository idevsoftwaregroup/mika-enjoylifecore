using Microsoft.EntityFrameworkCore;
using news.api.DependencyInjection;
using news.api.Settings;
using news.application.Contracts.Interfaces;
using news.application.Services;
using news.application.Settings;
using news.infrastructure.Core;
using news.infrastructure.Data;
using news.infrastructure.FileStorage;
using news.infrastructure.Messaging;
using news.infrastructure.Repositories;


var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<FileStorageSettings>(builder.Configuration.GetSection(FileStorageSettings.SECTION_NAME));
builder.Services.Configure<MessagingSettings>(builder.Configuration.GetSection(MessagingSettings.SECTION_NAME));
builder.Services.Configure<CoreSettings>(builder.Configuration.GetSection(CoreSettings.SECTION_NAME));

builder.Services.AddControllers();

builder.Services.Configure<JWTSettings>(builder.Configuration.GetSection(JWTSettings.SECTION_NAME));

builder.Services.AddScoped<INewsRepository, NewsRepository>();
builder.Services.AddScoped<INewsLocalFileStorageContext, NewsLocalFileStorageContext>();
builder.Services.AddScoped<INewsArticleService, NewsArticleService>();

builder.Services.AddScoped<ITempNewsRepository, TempNewsRepository>();
builder.Services.AddScoped<ITempNewsServices, TempNewsServices>();

builder.Services.AddDbContext<NewsContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

//builder.Services.AddHttpClient<IMessagingService, MessagingService>();
builder.Services.AddHttpClient<ICoreService, CoreServices>().ConfigurePrimaryHttpMessageHandler(() =>
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


builder.Services.AddJWTAuthenticationServices(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(policyBuilder =>
    policyBuilder.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyHeader())
);

var app = builder.Build();
app.UseCors();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{

    app.UseExceptionHandler("/error-development");

}
else
{
    app.UseExceptionHandler("/error");
}

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
