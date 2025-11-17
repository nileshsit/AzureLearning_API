using AzureLearning.API;
using AzureLearning.API.Configuration;
using AzureLearning.Model.Config;
using AzureLearning.Model.Settings;
using AzureLearning.Service.JWTAuthentication;
using AzureLearning.Service.Logger;
using NLog;

var builder = WebApplication.CreateBuilder(args);

// Logger Configuration
LogManager.Setup().LoadConfigurationFromFile(string.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));

// Add services to the container.

builder.Services.AddControllers();
RegisterService.RegisterServices(builder.Services);

// Application Setting & SMTP Settings Configuration read from appsettings.json
builder.Services.Configure<DataConfig>(builder.Configuration.GetSection("Data"));
builder.Services.AddSingleton<IJWTAuthenticationService, JWTAuthenticationService>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSingleton<ILoggerService, LoggerService>();

// Application Setting & SMTP Settings Configuration read from appsettings.json
builder.Services.Configure<SMTPSettings>(builder.Configuration.GetSection("SMTPSettings"));
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

// Add Open API config
builder.Services.AddOpenApiConfiguration(builder.Configuration);

// CORS configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AlowAllRequests", builder =>
    {
        builder
        .AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod();

    });
});

var app = builder.Build();

app.UseOpenApiConfiguration();

app.UseHttpsRedirection();

app.UseCors("AlowAllRequests");

app.UseAuthorization();

app.MapControllers();

app.Run();
