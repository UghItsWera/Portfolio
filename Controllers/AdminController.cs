using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortfolioCMS.Data;
using System.Security.Claims;
using PortfolioCMS.Models;

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

            var now = DateTime.UtcNow;
            var todayStart = now.Date;
            var monthStart = new DateTime(now.Year, now.Month, 1);

            var stats = new DashboardStats
            {
                TotalGames = _db.Projects.Count(p => p.Category == "game"),
                TotalWebsites = _db.Projects.Count(p => p.Category == "website"),
                TotalBooks = _db.Projects.Count(p => p.Category == "book"),
                TotalPublished = _db.Projects.Count(p => p.IsPublished),
                VisitsTotal = _db.PageVisits.Count(),
                VisitsToday = _db.PageVisits.Count(v => v.VisitedAt >= todayStart),
                VisitsThisMonth = _db.PageVisits.Count(v => v.VisitedAt >= monthStart)
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
// GET /Admin/Projects/Create
[HttpGet("Projects/Create")]
[Authorize]
public IActionResult Create(string category = "game")
{
    ViewData["ActiveNav"] = category + "s";
    ViewData["Category"] = category;
    return View("ProjectForm", new Project { Category = category });
}

// GET /Admin/Projects/Edit/{id}
[HttpGet("Projects/Edit/{id}")]
[Authorize]
public IActionResult Edit(int id)
{
    var project = _db.Projects.Find(id);
    if (project == null) return NotFound();

    ViewData["ActiveNav"] = project.Category + "s";
    ViewData["Category"] = project.Category;
    return View("ProjectForm", project);
}

// POST /Admin/Projects/Save
[HttpPost("Projects/Save")]
[Authorize]
[ValidateAntiForgeryToken]
public IActionResult Save(Project project)
{
    // Auto-generate slug from title if empty
    if (string.IsNullOrEmpty(project.Slug))
    {
        project.Slug = project.Title
            .ToLower()
            .Replace(" ", "-")
            .Replace("'", "")
            .Replace("\"", "")
            .Replace("&", "and");
    }

    if (project.Id == 0)
    {
        project.CreatedAt = DateTime.UtcNow;
        project.UpdatedAt = DateTime.UtcNow;
        _db.Projects.Add(project);
    }
    else
    {
        var existing = _db.Projects.Find(project.Id);
        if (existing == null) return NotFound();

        existing.Title = project.Title;
        existing.Slug = project.Slug;
        existing.Category = project.Category;
        existing.Summary = project.Summary;
        existing.Body = project.Body;
        existing.CoverImage = project.CoverImage;
        existing.Images = project.Images;
        existing.Buttons = project.Buttons;
        existing.Tags = project.Tags;
        existing.Metadata = project.Metadata;
        existing.IsPublished = project.IsPublished;
        existing.SortOrder = project.SortOrder;
        existing.UpdatedAt = DateTime.UtcNow;
    }

    _db.SaveChanges();

    return Redirect($"/Admin/{char.ToUpper(project.Category[0]) + project.Category[1..]}s");
}
// GET /Admin/Announcements
[HttpGet("Announcements")]
[Authorize]
public IActionResult Announcements()
{
    var announcements = _db.Announcements
        .OrderByDescending(a => a.CreatedAt)
        .ToList();

    ViewData["ActiveNav"] = "announcements";
    return View(announcements);
}

// GET /Admin/Announcements/Create
[HttpGet("Announcements/Create")]
[Authorize]
public IActionResult CreateAnnouncement()
{
    ViewData["ActiveNav"] = "announcements";
    return View("AnnouncementForm", new Announcement());
}

// GET /Admin/Announcements/Edit/{id}
[HttpGet("Announcements/Edit/{id}")]
[Authorize]
public IActionResult EditAnnouncement(int id)
{
    var announcement = _db.Announcements.Find(id);
    if (announcement == null) return NotFound();

    ViewData["ActiveNav"] = "announcements";
    return View("AnnouncementForm", announcement);
}

// POST /Admin/Announcements/Save
[HttpPost("Announcements/Save")]
[Authorize]
[ValidateAntiForgeryToken]
public IActionResult SaveAnnouncement(Announcement announcement)
{
    if (announcement.Id == 0)
    {
        announcement.CreatedAt = DateTime.UtcNow;
        _db.Announcements.Add(announcement);
    }
    else
    {
        var existing = _db.Announcements.Find(announcement.Id);
        if (existing == null) return NotFound();

        existing.Title = announcement.Title;
        existing.Body = announcement.Body;
    }

    _db.SaveChanges();
    return Redirect("/Admin/Announcements");
}

// POST /Admin/Announcements/Delete
[HttpPost("Announcements/Delete")]
[Authorize]
[ValidateAntiForgeryToken]
public IActionResult DeleteAnnouncement(int id)
{
    var announcement = _db.Announcements.Find(id);
    if (announcement != null)
    {
        _db.Announcements.Remove(announcement);
        _db.SaveChanges();
    }
    return Redirect("/Admin/Announcements");
}
    }
}