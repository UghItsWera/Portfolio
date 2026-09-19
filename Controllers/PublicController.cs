using Microsoft.AspNetCore.Mvc;
using PortfolioCMS.Data;
using PortfolioCMS.Helpers;
using PortfolioCMS.Models;

namespace PortfolioCMS.Controllers
{
    public class PublicController : Controller
    {
        private readonly AppDbContext _db;

        public PublicController(AppDbContext db)
        {
            _db = db;
        }

        // ============================================================
        // HOME
        // ============================================================

        // GET /
        public IActionResult Index()
        {
            var recentGames = _db.Projects
                .Where(p => p.Category == "game" && p.IsPublished)
                .OrderBy(p => p.SortOrder)
                .ThenByDescending(p => p.UpdatedAt)
                .Take(3)
                .ToList();

            var recentBooks = _db.Projects
                .Where(p => p.Category == "book" && p.IsPublished)
                .OrderBy(p => p.SortOrder)
                .ThenByDescending(p => p.UpdatedAt)
                .Take(3)
                .ToList();

            var recentWebsites = _db.Projects
                .Where(p => p.Category == "website" && p.IsPublished)
                .OrderBy(p => p.SortOrder)
                .ThenByDescending(p => p.UpdatedAt)
                .Take(3)
                .ToList();

            var announcements = _db.Announcements
                .OrderByDescending(a => a.CreatedAt)
                .Take(5)
                .ToList();

            ViewBag.RecentGames = recentGames;
            ViewBag.RecentBooks = recentBooks;
            ViewBag.RecentWebsites = recentWebsites;
            ViewBag.Announcements = announcements;

            // Load category themes for the homepage.
            ViewBag.GameTheme = GetCategoryTheme("game");
            ViewBag.WebsiteTheme = GetCategoryTheme("website");
            ViewBag.BookTheme = GetCategoryTheme("book");

            return View();
        }

        // ============================================================
        // ABOUT
        // ============================================================

        // GET /about
        public IActionResult About()
        {
            var about = _db.AboutContent.FirstOrDefault()
                        ?? new AboutContent();

            return View(about);
        }

        // ============================================================
        // CONTACT
        // ============================================================

        // POST /about/contact
        [HttpPost]
        [Route("about/contact")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Contact(
            string fullName,
            string email,
            string message)
        {
            try
            {
                var emailService = HttpContext.RequestServices
                    .GetRequiredService<PortfolioCMS.Services.EmailService>();

                await emailService.SendContactEmailAsync(
                    fullName,
                    email,
                    message
                );

                TempData["ContactSuccess"] =
                    "Your message has been sent. I'll be in touch.";
            }
            catch (Exception ex)
            {
                TempData["ContactError"] =
                    $"Error: {ex.Message}";
            }

            return RedirectToAction("About");
        }

        // ============================================================
        // GAMES
        // ============================================================

        // GET /games
        public IActionResult Games()
        {
            var games = _db.Projects
                .Where(p => p.Category == "game" && p.IsPublished)
                .OrderBy(p => p.SortOrder)
                .ThenByDescending(p => p.CreatedAt)
                .ToList();

            ViewBag.CategoryTheme = GetCategoryTheme("game");

            return View(games);
        }

        // GET /games/{slug}
        public IActionResult Game(string slug)
        {
            var project = _db.Projects
                .FirstOrDefault(p =>
                    p.Slug == slug &&
                    p.Category == "game" &&
                    p.IsPublished);

            if (project == null)
                return NotFound();

            ViewBag.CategoryTheme = GetCategoryTheme("game");

            return View(
                ProjectDisplayHelper.BuildDetailViewModel(project)
            );
        }

        // ============================================================
        // BOOKS
        // ============================================================

        // GET /books
        public IActionResult Books()
        {
            var books = _db.Projects
                .Where(p => p.Category == "book" && p.IsPublished)
                .OrderBy(p => p.SortOrder)
                .ThenByDescending(p => p.CreatedAt)
                .ToList();

            ViewBag.CategoryTheme = GetCategoryTheme("book");

            return View(books);
        }

        // GET /books/{slug}
        public IActionResult Book(string slug)
        {
            var project = _db.Projects
                .FirstOrDefault(p =>
                    p.Slug == slug &&
                    p.Category == "book" &&
                    p.IsPublished);

            if (project == null)
                return NotFound();

            ViewBag.CategoryTheme = GetCategoryTheme("book");

            return View(
                ProjectDisplayHelper.BuildDetailViewModel(project)
            );
        }

        // ============================================================
        // WEBSITES
        // ============================================================

        // GET /websites
        public IActionResult Websites()
        {
            var websites = _db.Projects
                .Where(p => p.Category == "website" && p.IsPublished)
                .OrderBy(p => p.SortOrder)
                .ThenByDescending(p => p.CreatedAt)
                .ToList();

            ViewBag.CategoryTheme = GetCategoryTheme("website");

            return View(websites);
        }

        // GET /websites/{slug}
        public IActionResult Website(string slug)
        {
            var project = _db.Projects
                .FirstOrDefault(p =>
                    p.Slug == slug &&
                    p.Category == "website" &&
                    p.IsPublished);

            if (project == null)
                return NotFound();

            ViewBag.CategoryTheme = GetCategoryTheme("website");

            return View(
                ProjectDisplayHelper.BuildDetailViewModel(project)
            );
        }

        // ============================================================
        // MISCELLANEOUS
        // ============================================================

        // GET /misc
        public IActionResult Miscellaneous()
        {
            var projects = _db.Projects
                .Where(p => p.Category == "misc" && p.IsPublished)
                .OrderBy(p => p.SortOrder)
                .ThenByDescending(p => p.CreatedAt)
                .ToList();

            ViewBag.CategoryTheme = GetCategoryTheme("misc");

            return View(projects);
        }

        // GET /misc/{slug}
        public IActionResult MiscProject(string slug)
        {
            var project = _db.Projects
                .FirstOrDefault(p =>
                    p.Slug == slug &&
                    p.Category == "misc" &&
                    p.IsPublished);

            if (project == null)
                return NotFound();

            ViewBag.CategoryTheme = GetCategoryTheme("misc");

            return View(
                ProjectDisplayHelper.BuildDetailViewModel(project)
            );
        }

        // ============================================================
        // CATEGORY THEME HELPER
        // ============================================================

        private CategoryTheme GetCategoryTheme(string category)
        {
            var theme = _db.CategoryThemes
                .FirstOrDefault(t => t.Category == category);

            // Fallback in case a theme does not exist yet.
            // This prevents the public site from breaking.
            return theme ?? CreateFallbackCategoryTheme(category);
        }

        private static CategoryTheme CreateFallbackCategoryTheme(
            string category)
        {
            return category switch
            {
                "game" => new CategoryTheme
                {
                    Category = "game",

                    LightBackground = "#D0DDC4",
                    LightAccent = "#4F6A4C",
                    LightCard = "#E1E9D9",

                    DarkBackground = "#263128",
                    DarkAccent = "#AFC7A7",
                    DarkCard = "#344238"
                },

                "website" => new CategoryTheme
                {
                    Category = "website",

                    LightBackground = "#B8C7D9",
                    LightAccent = "#52677F",
                    LightCard = "#D5DEEA",

                    DarkBackground = "#263442",
                    DarkAccent = "#AFC4D8",
                    DarkCard = "#354452"
                },

                "book" => new CategoryTheme
                {
                    Category = "book",

                    LightBackground = "#D8B9C5",
                    LightAccent = "#795260",
                    LightCard = "#EAD8DE",

                    DarkBackground = "#3A2931",
                    DarkAccent = "#D5AEBB",
                    DarkCard = "#4B3740"
                },

                "misc" => new CategoryTheme
                {
                    Category = "misc",

                    LightBackground = "#D6C7B5",
                    LightAccent = "#6E5B45",
                    LightCard = "#E7DED1",

                    DarkBackground = "#352F29",
                    DarkAccent = "#CBB99E",
                    DarkCard = "#484036"
                },

                _ => new CategoryTheme
                {
                    Category = category,

                    LightBackground = "#E0E4E7",
                    LightAccent = "#52677F",
                    LightCard = "#D5DEEA",

                    DarkBackground = "#263442",
                    DarkAccent = "#AFC4D8",
                    DarkCard = "#354452"
                }
            };
        }
    }
}