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

        // ============================================================
        // AUTHENTICATION
        // ============================================================

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

                var identity = new ClaimsIdentity(
                    claims,
                    CookieAuthenticationDefaults.AuthenticationScheme
                );

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(identity)
                );

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
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            return RedirectToAction("Login");
        }

        // ============================================================
        // DASHBOARD
        // ============================================================

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

        // ============================================================
        // CATEGORY PROJECT LISTS
        // ============================================================

        // GET /Admin/Games
        [HttpGet("Games")]
        [Authorize]
        public IActionResult Games() =>
            ProjectList(
                "game",
                "games",
                "Games",
                "Manage your game projects and interactive experiences."
            );

        // GET /Admin/Websites
        [HttpGet("Websites")]
        [Authorize]
        public IActionResult Websites() =>
            ProjectList(
                "website",
                "websites",
                "Websites",
                "Manage your web portfolio and digital projects."
            );

        // GET /Admin/Books
        [HttpGet("Books")]
        [Authorize]
        public IActionResult Books() =>
            ProjectList(
                "book",
                "books",
                "Books",
                "Manage your published manuscripts and written works."
            );

        // GET /Admin/Miscellaneous
        [HttpGet("Miscellaneous")]
        [Authorize]
        public IActionResult Miscellaneous() =>
            ProjectList(
                "misc",
                "miscellaneous",
                "Miscellaneous",
                "Curiosities, experiments, and everything in between."
            );

        // ============================================================
        // PROJECT ACTIONS
        // ============================================================

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
            ViewData["ActiveNav"] = ActiveNavForCategory(category);
            ViewData["Category"] = category;

            return View(
                "ProjectForm",
                new Project { Category = category }
            );
        }

        // GET /Admin/Projects/Edit/{id}
        [HttpGet("Projects/Edit/{id}")]
        [Authorize]
        public IActionResult Edit(int id)
        {
            var project = _db.Projects.Find(id);

            if (project == null)
                return NotFound();

            ViewData["ActiveNav"] = ActiveNavForCategory(project.Category);
            ViewData["Category"] = project.Category;

            return View("ProjectForm", project);
        }

        // POST /Admin/Projects/Save
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

                if (existing == null)
                    return NotFound();

                CopyProjectFields(existing, project);
                existing.UpdatedAt = DateTime.UtcNow;
            }

            _db.SaveChanges();

            return Redirect(CategoryListUrl(project.Category));
        }

        // ============================================================
        // CATEGORY THEMES
        // ============================================================

        // POST /Admin/CategoryTheme/Save
        [HttpPost("CategoryTheme/Save")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public IActionResult SaveCategoryTheme(
            CategoryTheme theme,
            string returnUrl)
        {
            if (string.IsNullOrWhiteSpace(theme.Category))
                return BadRequest();

            var existing = _db.CategoryThemes
                .FirstOrDefault(t => t.Category == theme.Category);

            if (existing == null)
            {
                _db.CategoryThemes.Add(theme);
            }
            else
            {
                existing.LightBackground = theme.LightBackground;
                existing.LightAccent = theme.LightAccent;
                existing.LightCard = theme.LightCard;

                existing.DarkBackground = theme.DarkBackground;
                existing.DarkAccent = theme.DarkAccent;
                existing.DarkCard = theme.DarkCard;
            }

            _db.SaveChanges();

            TempData["ThemeSuccess"] =
                $"{GetCategoryLabel(theme.Category)} colours saved.";

            return Redirect(
                returnUrl ?? CategoryListUrl(theme.Category)
            );
        }

        // ============================================================
        // ANNOUNCEMENTS
        // ============================================================

        // GET /Admin/Announcements
        [HttpGet("Announcements")]
        [Authorize]
        public IActionResult Announcements()
        {
            ViewData["ActiveNav"] = "announcements";

            return View(
                _db.Announcements
                    .OrderByDescending(a => a.CreatedAt)
                    .ToList()
            );
        }

        // GET /Admin/Announcements/Create
        [HttpGet("Announcements/Create")]
        [Authorize]
        public IActionResult CreateAnnouncement()
        {
            ViewData["ActiveNav"] = "announcements";

            return View(
                "AnnouncementForm",
                new Announcement()
            );
        }

        // GET /Admin/Announcements/Edit/{id}
        [HttpGet("Announcements/Edit/{id}")]
        [Authorize]
        public IActionResult EditAnnouncement(int id)
        {
            var announcement = _db.Announcements.Find(id);

            if (announcement == null)
                return NotFound();

            ViewData["ActiveNav"] = "announcements";

            return View("AnnouncementForm", announcement);
        }

        // POST /Admin/Announcements/Save
        [HttpPost("Announcements/Save")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public IActionResult SaveAnnouncement(
            Announcement announcement)
        {
            if (announcement.Id == 0)
            {
                announcement.CreatedAt = DateTime.UtcNow;

                _db.Announcements.Add(announcement);
            }
            else
            {
                var existing = _db.Announcements
                    .Find(announcement.Id);

                if (existing == null)
                    return NotFound();

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

        // ============================================================
        // ABOUT
        // ============================================================

        // GET /Admin/About
        [HttpGet("About")]
        [Authorize]
        public IActionResult About()
        {
            ViewData["ActiveNav"] = "about";

            return View(
                "AboutForm",
                _db.AboutContent.FirstOrDefault()
                    ?? new AboutContent()
            );
        }

        // POST /Admin/About/Save
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
                existing.EmailAddress = content.EmailAddress;
                existing.Location = content.Location;
                existing.Github = content.Github;
                existing.GithubLink = content.GithubLink;
            }

            _db.SaveChanges();

            TempData["Success"] = "About page saved.";

            return Redirect("/Admin/About");
        }

        // ============================================================
        // PRIVATE HELPERS
        // ============================================================

        private IActionResult ProjectList(
            string category,
            string activeNav,
            string label,
            string description)
        {
            var projects = _db.Projects
                .Where(p => p.Category == category)
                .OrderBy(p => p.SortOrder)
                .ThenByDescending(p => p.UpdatedAt)
                .ToList();

            var theme = GetOrCreateCategoryTheme(category);

            ViewData["ActiveNav"] = activeNav;
            ViewData["Category"] = category;
            ViewData["CategoryLabel"] = label;
            ViewData["CategoryDescription"] = description;
            ViewData["CategoryTheme"] = theme;

            return View("ProjectList", projects);
        }

        private CategoryTheme GetOrCreateCategoryTheme(
            string category)
        {
            var theme = _db.CategoryThemes
                .FirstOrDefault(t => t.Category == category);

            if (theme != null)
                return theme;

            theme = CreateDefaultCategoryTheme(category);

            _db.CategoryThemes.Add(theme);
            _db.SaveChanges();

            return theme;
        }

        private static CategoryTheme CreateDefaultCategoryTheme(
            string category)
        {
            return category switch
            {
                "game" => new CategoryTheme
                {
                    Category = "game",

                    // Light mode
                    LightBackground = "#D0DDC4",
                    LightAccent = "#4F6A4C",
                    LightCard = "#E1E9D9",

                    // Dark mode
                    DarkBackground = "#263128",
                    DarkAccent = "#AFC7A7",
                    DarkCard = "#344238"
                },

                "website" => new CategoryTheme
                {
                    Category = "website",

                    // Light mode
                    LightBackground = "#B8C7D9",
                    LightAccent = "#52677F",
                    LightCard = "#D5DEEA",

                    // Dark mode
                    DarkBackground = "#263442",
                    DarkAccent = "#AFC4D8",
                    DarkCard = "#354452"
                },

                "book" => new CategoryTheme
                {
                    Category = "book",

                    // Light mode
                    LightBackground = "#D8B9C5",
                    LightAccent = "#795260",
                    LightCard = "#EAD8DE",

                    // Dark mode
                    DarkBackground = "#3A2931",
                    DarkAccent = "#D5AEBB",
                    DarkCard = "#4B3740"
                },

                "misc" => new CategoryTheme
                {
                    Category = "misc",

                    // Light mode
                    LightBackground = "#D6C7B5",
                    LightAccent = "#6E5B45",
                    LightCard = "#E7DED1",

                    // Dark mode
                    DarkBackground = "#352F29",
                    DarkAccent = "#CBB99E",
                    DarkCard = "#484036"
                },

                _ => new CategoryTheme
                {
                    Category = category,

                    // Light mode
                    LightBackground = "#E0E4E7",
                    LightAccent = "#52677F",
                    LightCard = "#D5DEEA",

                    // Dark mode
                    DarkBackground = "#263442",
                    DarkAccent = "#AFC4D8",
                    DarkCard = "#354452"
                }
            };
        }

        private static void CopyProjectFields(
            Project target,
            Project source)
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
            title
                .ToLower()
                .Replace(" ", "-")
                .Replace("'", "")
                .Replace("\"", "")
                .Replace("&", "and");

        private static string ActiveNavForCategory(
            string category) =>
            category switch
            {
                "misc" => "miscellaneous",
                _ => category + "s"
            };

        private static string CategoryListUrl(
            string category) =>
            category switch
            {
                "misc" => "/Admin/Miscellaneous",
                _ => $"/Admin/{char.ToUpper(category[0])}{category[1..]}s"
            };

        private static string GetCategoryLabel(
            string category)
        {
            return category switch
            {
                "game" => "Games",
                "website" => "Websites",
                "book" => "Books",
                "misc" => "Miscellaneous",
                _ => category
            };
        }
    }
}