using FluentValidation;
using FluentValidation.AspNetCore;

using Microsoft.EntityFrameworkCore;

using Serilog;
using Serilog.Events;

using WebApi;
using WebApi.Data;
using WebApi.DTOs.Validation;

var builder = WebApplication.CreateBuilder(args);

var outputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}Environment: {Environment}{NewLine}{Exception}";

Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .MinimumLevel.Debug()
                .Enrich.WithThreadName()
                .Enrich.WithProperty("Environment", builder.Environment.EnvironmentName)
                .WriteTo.Console(outputTemplate: outputTemplate)
                //.WriteTo.File("logs/myapp.txt", 
                //    rollingInterval: RollingInterval.Day,
                //    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}ThreadId: {ThreadId}{NewLine}{Exception}")
                .CreateLogger();

//Log.Logger = new LoggerConfiguration()
//    .ReadFrom.Configuration(builder.Configuration)
//    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddDbContext<TodoDbContext>(options => {
    options.UseSqlServer(builder.Configuration.GetConnectionString("TodoDbConnectionString"));
});

//builder.Services.AddOutputCache();
builder.Services.AddMemoryCache(); // singleton

//builder.Services.AddDistributedMemoryCache(s => {
//    
//});

builder.Services.AddControllers();

builder.Services.AddAuthenticationAndAuthorization(builder.Configuration);
builder.Services.AddLogging(builder.Configuration);
builder.Services.AddSwagger();

builder.Services.AddDomainDependencies();


builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseOutputCache();
app.UseResponseCaching();

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.Run();


// Application Insights
// MSSQL
// MongoDB
// Console
// File
// ElasticSearch

/*
 + Authn/Authz -> Identity, JWT -> Policies, Identity, RefreshToken
 + Validation -> FluentValidation
 + Logging -> Microsoft.Extensions.Logging, Serilog
 + Caching -> Memory Cache
 - Cron jobs -> Quartz.NET
 - Testing -> xUnit, Moq
 - Secret Management -> GitHub Secrets
*/


// GET /api/orders

// cache
// -> 200 - [{}, {}, {}]