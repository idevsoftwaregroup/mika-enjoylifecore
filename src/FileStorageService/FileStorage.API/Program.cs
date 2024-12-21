using FileStorage.API.Infrastructure;
using FileStorage.API.Services;
using FileStorage.API.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.Configure<FileStorageSettings>(builder.Configuration.GetSection(FileStorageSettings.SECTION_NAME));

builder.Services.AddScoped<IFileStorageService, FileStorageService>();
builder.Services.AddDbContext<FileStorageDataContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(policyBuilder =>
    policyBuilder.AddDefaultPolicy(policy =>
        policy.WithOrigins("*").AllowAnyHeader().AllowAnyHeader())
);
var app = builder.Build();
app.UseCors();
Directory.CreateDirectory(Directory.GetCurrentDirectory() + @"FileStorage");
app.UseStaticFiles(new StaticFileOptions()
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"FileStorage")),
    RequestPath = new PathString("/MikaFiles")
});
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
