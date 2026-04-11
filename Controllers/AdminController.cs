using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortfolioCMS.Data;
using System.Security.Claims;

namespace PortfolioCMS.Controllers
{
    [Route("Admin")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _config;

        public AdminController(AppDbContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        // GET /Admin/Login
        [HttpGet("Login")]
        public IActionResult Login()
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Dashboard");

            return View();
        }

        // POST /Admin/Login
        [HttpPost("Login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password)
        {
            var validUsername = _config["AdminCredentials:Username"];
            var validPassword = _config["AdminCredentials:Password"];

            if (username == validUsername && password == validPassword)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.Role, "Admin")
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                return RedirectToAction("Dashboard");
            }

            ViewBag.Error = "Invalid username or password.";
            return View();
        }

        // POST /Admin/Logout
        [HttpPost("Logout")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        // GET /Admin/Dashboard
        [HttpGet("Dashboard")]
        [Authorize]
        public IActionResult Dashboard()
        {
            var recentProjects = _db.Projects
                .OrderByDescending(p => p.UpdatedAt)
                .Take(5)
                .ToList();

            var announcements = _db.Announcements
                .OrderByDescending(a => a.CreatedAt)
                .Take(3)
                .ToList();

            var stats = new
            {
                TotalGames = _db.Projects.Count(p => p.Category == "game"),
                TotalWebsites = _db.Projects.Count(p => p.Category == "website"),
                TotalBooks = _db.Projects.Count(p => p.Category == "book"),
                TotalPublished = _db.Projects.Count(p => p.IsPublished)
            };

            ViewBag.RecentProjects = recentProjects;
            ViewBag.Announcements = announcements;
            ViewBag.Stats = stats;

            return View();
        }
        // GET /Admin/Games
[HttpGet("Games")]
[Authorize]
public IActionResult Games()
{
    var projects = _db.Projects
        .Where(p => p.Category == "game")
        .OrderBy(p => p.SortOrder)
        .ThenByDescending(p => p.UpdatedAt)
        .ToList();

    ViewData["ActiveNav"] = "games";
    ViewData["Category"] = "game";
    ViewData["CategoryLabel"] = "Games";
    ViewData["CategoryDescription"] = "Manage your game projects and interactive experiences.";
    return View("ProjectList", projects);
}

// GET /Admin/Websites
[HttpGet("Websites")]
[Authorize]
public IActionResult Websites()
{
    var projects = _db.Projects
        .Where(p => p.Category == "website")
        .OrderBy(p => p.SortOrder)
        .ThenByDescending(p => p.UpdatedAt)
        .ToList();

    ViewData["ActiveNav"] = "websites";
    ViewData["Category"] = "website";
    ViewData["CategoryLabel"] = "Websites";
    ViewData["CategoryDescription"] = "Manage your web portfolio and digital projects.";
    return View("ProjectList", projects);
}

// GET /Admin/Books
[HttpGet("Books")]
[Authorize]
public IActionResult Books()
{
    var projects = _db.Projects
        .Where(p => p.Category == "book")
        .OrderBy(p => p.SortOrder)
        .ThenByDescending(p => p.UpdatedAt)
        .ToList();

    ViewData["ActiveNav"] = "books";
    ViewData["Category"] = "book";
    ViewData["CategoryLabel"] = "Books";
    ViewData["CategoryDescription"] = "Manage your published manuscripts and written works.";
    return View("ProjectList", projects);
}

// POST /Admin/Projects/TogglePublish
[HttpPost("Projects/TogglePublish")]
[Authorize]
[ValidateAntiForgeryToken]
public IActionResult TogglePublish(int id, string returnUrl)
{
    var project = _db.Projects.Find(id);
    if (project != null)
    {
        project.IsPublished = !project.IsPublished;
        project.UpdatedAt = DateTime.UtcNow;
        _db.SaveChanges();
    }
    return Redirect(returnUrl ?? "/Admin/Dashboard");
}

// POST /Admin/Projects/Delete
[HttpPost("Projects/Delete")]
[Authorize]
[ValidateAntiForgeryToken]
public IActionResult Delete(int id, string returnUrl)
{
    var project = _db.Projects.Find(id);
    if (project != null)
    {
        _db.Projects.Remove(project);
        _db.SaveChanges();
    }
    return Redirect(returnUrl ?? "/Admin/Dashboard");
}
    }
}