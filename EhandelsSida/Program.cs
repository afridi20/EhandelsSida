using EhandelsSida.Data;
using EhandelsSida.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
 

var builder = WebApplication.CreateBuilder(args);

// 1. Lägg till databasen (SQL Server LocalDB)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Lägg till Identity
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false; // ändra till true om du vill kräva e-postbekräftelse
})
.AddEntityFrameworkStores<ApplicationDbContext>();

// 3. Lägg till MVC Controllers med Views
builder.Services.AddControllersWithViews();

var app = builder.Build();

// 4. Middleware (HTTP pipeline)
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Viktigt för Identity
app.UseAuthorization();

// 5. Standard routing för controllers
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// 6. Identity Razor Pages routing
app.MapRazorPages();

app.Run();
