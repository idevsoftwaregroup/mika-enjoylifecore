
using Microsoft.EntityFrameworkCore;
using OrganizationChart.API.Contracts.Interfaces;
using OrganizationChart.API.Controllers;
using OrganizationChart.API.Infrastructure.FileStorage;
using OrganizationChart.API.Infrastructure.Persistence;
using OrganizationChart.API.Services;
using OrganizationChart.API.Settings;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
       .AddJsonOptions(
            options => options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve
    ) ;
builder.Services.Configure<FileStorageSettings>(builder.Configuration.GetSection(FileStorageSettings.SECTION_NAME));
builder.Services.AddDbContext<OrganizationChartDataContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddScoped<IOrganizationChartService, OrganizationChartService>();
builder.Services.AddScoped<ISystemDetails, GetSystemDetailsAsync>();
builder.Services.AddScoped<ISystemUpsRepository, SystemUpsRepository>();
builder.Services.AddHttpClient<IFileStorageService, FileStorageService>();
builder.Services.AddCors(policyBuilder =>
    policyBuilder.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod())
);


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.UseAuthorization();

app.MapControllers();

app.Run();
