using Microsoft.EntityFrameworkCore;
using HattmakarenWebbAppGrupp03.Data;
using HattmakarenWebbAppGrupp03.Models;

var builder = WebApplication.CreateBuilder(args);

// 1. Hämta Connection String från appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// 2. Registrera ApplicationDbContext att använda SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// 3. Lägg till tjänster för MVC (Controllers och Views)
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Konfigurera HTTP-pipelinen (Hanterar fel, CSS, Routing osv)
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Bestämmer hur URL-adresser ska tolkas
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();