using Assig1;
using Microsoft.EntityFrameworkCore;

// Create Application
var builder = WebApplication.CreateBuilder(args);
Debug.SetActive(builder);


// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();

// Set DB Context
builder.Services.AddDbContext<Assig1.Data.EnvDataContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("EnvData") ??
throw new InvalidOperationException("Connection String for EnvData DB not found")));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
