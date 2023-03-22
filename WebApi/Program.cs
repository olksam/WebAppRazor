using Microsoft.EntityFrameworkCore;

using WebApi;
using WebApi.Data;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<TodoDbContext>(options => {
    options.UseSqlServer(builder.Configuration.GetConnectionString("TodoDbConnectionString"));
});

builder.Services.AddControllers();

builder.Services.AddAuthenticationAndAuthorization(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwagger();

builder.Services.AddDomainDependencies();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();

app.UseAuthentication();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.Run();

/*

 + Authn/Authz -> Identity, JWT -> Policies, Identity, RefreshToken
 - Validation -> FluentValidation
 - Logging -> Serilog
 - Caching -> Memory Cache
 - Cron jobs -> Quartz.NET
 - Testing -> xUnit, Moq
 - Secret Management -> GitHub Secrets
*/