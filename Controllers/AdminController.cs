using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortfolioCMS.Data;
using PortfolioCMS.Models;
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
                    new(ClaimTypes.Name, username),
                    new(ClaimTypes.Role, "Admin")
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

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
            var now = DateTime.UtcNow;
            var todayStart = now.Date;
            var monthStart = new DateTime(now.Year, now.Month, 1);

            ViewBag.RecentProjects = _db.Projects
                .OrderByDescending(p => p.UpdatedAt)
                .Take(5)
                .ToList();

            ViewBag.Announcements = _db.Announcements
                .OrderByDescending(a => a.CreatedAt)
                .Take(3)
                .ToList();

            ViewBag.Stats = new DashboardStats
            {
                TotalGames = _db.Projects.Count(p => p.Category == "game"),
                TotalWebsites = _db.Projects.Count(p => p.Category == "website"),
                TotalBooks = _db.Projects.Count(p => p.Category == "book"),
                TotalPublished = _db.Projects.Count(p => p.IsPublished),
                VisitsTotal = _db.PageVisits.Count(),
                VisitsToday = _db.PageVisits.Count(v => v.VisitedAt >= todayStart),
                VisitsThisMonth = _db.PageVisits.Count(v => v.VisitedAt >= monthStart)
            };

            return View();
        }

        [HttpGet("Games")]
        [Authorize]
        public IActionResult Games() =>
            ProjectList("game", "games", "Games", "Manage your game projects and interactive experiences.");

        [HttpGet("Websites")]
        [Authorize]
        public IActionResult Websites() =>
            ProjectList("website", "websites", "Websites", "Manage your web portfolio and digital projects.");

        [HttpGet("Books")]
        [Authorize]
        public IActionResult Books() =>
            ProjectList("book", "books", "Books", "Manage your published manuscripts and written works.");

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

        [HttpGet("Projects/Create")]
        [Authorize]
        public IActionResult Create(string category = "game")
        {
            ViewData["ActiveNav"] = category + "s";
            ViewData["Category"] = category;
            return View("ProjectForm", new Project { Category = category });
        }

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

        [HttpPost("Projects/Save")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public IActionResult Save(Project project)
        {
            if (string.IsNullOrEmpty(project.Slug))
                project.Slug = GenerateSlug(project.Title);

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

                CopyProjectFields(existing, project);
                existing.UpdatedAt = DateTime.UtcNow;
            }

            _db.SaveChanges();
            return Redirect(CategoryListUrl(project.Category));
        }

        [HttpGet("Announcements")]
        [Authorize]
        public IActionResult Announcements()
        {
            ViewData["ActiveNav"] = "announcements";
            return View(_db.Announcements.OrderByDescending(a => a.CreatedAt).ToList());
        }

        [HttpGet("Announcements/Create")]
        [Authorize]
        public IActionResult CreateAnnouncement()
        {
            ViewData["ActiveNav"] = "announcements";
            return View("AnnouncementForm", new Announcement());
        }

        [HttpGet("Announcements/Edit/{id}")]
        [Authorize]
        public IActionResult EditAnnouncement(int id)
        {
            var announcement = _db.Announcements.Find(id);
            if (announcement == null) return NotFound();

            ViewData["ActiveNav"] = "announcements";
            return View("AnnouncementForm", announcement);
        }

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

        [HttpGet("About")]
        [Authorize]
        public IActionResult About()
        {
            ViewData["ActiveNav"] = "about";
            return View("AboutForm", _db.AboutContent.FirstOrDefault() ?? new AboutContent());
        }

        [HttpPost("About/Save")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public IActionResult SaveAbout(AboutContent content)
        {
            var existing = _db.AboutContent.FirstOrDefault();
            if (existing == null)
            {
                _db.AboutContent.Add(content);
            }
            else
            {
                existing.ProfileImage = content.ProfileImage;
                existing.Bio1 = content.Bio1;
                existing.Bio2 = content.Bio2;
                existing.Bio3 = content.Bio3;
                existing.LinkedIn = content.LinkedIn;
                existing.LinkedInLink = content.LinkedInLink;
                existing.CV = content.CV;
                existing.CVDownload = content.CVDownload;
                existing.MediumLabel = content.MediumLabel;
                existing.MediumValue = content.MediumValue;
                existing.EmailAddress = content.EmailAddress;
                existing.Location = content.Location;
            }

            _db.SaveChanges();
            TempData["Success"] = "About page saved.";
            return Redirect("/Admin/About");
        }

        private IActionResult ProjectList(string category, string activeNav, string label, string description)
        {
            var projects = _db.Projects
                .Where(p => p.Category == category)
                .OrderBy(p => p.SortOrder)
                .ThenByDescending(p => p.UpdatedAt)
                .ToList();

            ViewData["ActiveNav"] = activeNav;
            ViewData["Category"] = category;
            ViewData["CategoryLabel"] = label;
            ViewData["CategoryDescription"] = description;
            return View("ProjectList", projects);
        }

        private static void CopyProjectFields(Project target, Project source)
        {
            target.Title = source.Title;
            target.Slug = source.Slug;
            target.Category = source.Category;
            target.Summary = source.Summary;
            target.Body = source.Body;
            target.CoverImage = source.CoverImage;
            target.Images = source.Images;
            target.Buttons = source.Buttons;
            target.Tags = source.Tags;
            target.Metadata = source.Metadata;
            target.IsPublished = source.IsPublished;
            target.SortOrder = source.SortOrder;
        }

        private static string GenerateSlug(string title) =>
            title.ToLower()
                .Replace(" ", "-")
                .Replace("'", "")
                .Replace("\"", "")
                .Replace("&", "and");

        private static string CategoryListUrl(string category) =>
            $"/Admin/{char.ToUpper(category[0])}{category[1..]}s";
    }
}
