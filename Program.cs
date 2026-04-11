using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using PortfolioCMS.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Auth
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Admin/Login";
        options.LogoutPath = "/Admin/Logout";
        options.AccessDeniedPath = "/Admin/Login";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
    });

builder.Services.AddAuthorization();
builder.Services.AddSassCompiler();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseMiddleware<PortfolioCMS.Middleware.VisitTrackingMiddleware>();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
// Short public URLs (must be registered before the generic {controller}/{action} route)
app.MapControllerRoute(name: "public-about", pattern: "about", defaults: new { controller = "Public", action = "About" });
app.MapControllerRoute(name: "public-games", pattern: "games", defaults: new { controller = "Public", action = "Games" });
app.MapControllerRoute(name: "public-books", pattern: "books", defaults: new { controller = "Public", action = "Books" });
app.MapControllerRoute(name: "public-websites", pattern: "websites", defaults: new { controller = "Public", action = "Websites" });

app.MapControllerRoute(name: "game-detail", pattern: "games/{slug}", defaults: new { controller = "Public", action = "Game" });
app.MapControllerRoute(name: "book-detail", pattern: "books/{slug}", defaults: new { controller = "Public", action = "Book" });
app.MapControllerRoute(name: "website-detail", pattern: "websites/{slug}", defaults: new { controller = "Public", action = "Website" });

app.MapControllerRoute(name: "default", pattern: "{controller=Public}/{action=Index}/{id?}");

app.Run();