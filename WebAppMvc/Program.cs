using Microsoft.EntityFrameworkCore;

using WebAppMvc.Data;
using WebAppMvc.Filters;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<WebAppMvcContext>(options =>
    //options.UseSqlServer(builder.Configuration.GetConnectionString("WebAppMvcContext") 
    //?? throw new InvalidOperationException("Connection string 'WebAppMvcContext' not found."))
    options.UseInMemoryDatabase("tempdb")

    );

// Add services to the container.
builder.Services.AddControllersWithViews(o => o.Filters.Add(typeof (GlobalExceptionFilter)));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment()) {
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
