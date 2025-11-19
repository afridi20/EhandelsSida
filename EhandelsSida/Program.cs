using EhandelsSida.Data;
using EhandelsSida.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// 1. L�gg till databasen (SQL Server LocalDB)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. L�gg till Identity
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false; // anv�ndare kan logga in direkt
})
    .AddEntityFrameworkStores<ApplicationDbContext>();

// 3. L�gg till MVC (Controllers + Views)
builder.Services.AddControllersWithViews();

var app = builder.Build();

// 4. Middleware pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Viktigt: Identity kr�ver Authentication + Authorization
app.UseAuthentication();
app.UseAuthorization();

// Standard routing
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Identity endpoints (t.ex. /Account/Login)
app.MapRazorPages();

app.Run();
