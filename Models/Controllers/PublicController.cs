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

            return View();
        }
        // GET /about
        public IActionResult About()
        {
            var about = _db.AboutContent.FirstOrDefault() ?? new AboutContent();
            return View(about);
        }

        [HttpPost]
[Route("about/contact")]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Contact(string fullName, string email, string message)
{
    try
    {
        var emailService = HttpContext.RequestServices
            .GetRequiredService<PortfolioCMS.Services.EmailService>();
        await emailService.SendContactEmailAsync(fullName, email, message);
        TempData["ContactSuccess"] = "Your message has been sent. I'll be in touch.";
    }
    catch (Exception ex)
    {
        TempData["ContactError"] = $"Error: {ex.Message}";
    }

    return RedirectToAction("About");
}
        // GET /games
        public IActionResult Games()
        {
            var games = _db.Projects
                .Where(p => p.Category == "game" && p.IsPublished)
                .OrderBy(p => p.SortOrder)
                .ThenByDescending(p => p.CreatedAt)
                .ToList();

            return View(games);
        }

        // GET /games/{slug}
        public IActionResult Game(string slug)
        {
            var project = _db.Projects
                .FirstOrDefault(p => p.Slug == slug && p.Category == "game" && p.IsPublished);

            if (project == null) return NotFound();

            return View(ProjectDisplayHelper.BuildDetailViewModel(project));
        }

        // GET /books
        public IActionResult Books()
        {
            var books = _db.Projects
                .Where(p => p.Category == "book" && p.IsPublished)
                .OrderBy(p => p.SortOrder)
                .ThenByDescending(p => p.CreatedAt)
                .ToList();

            return View(books);
        }

        // GET /books/{slug}
        public IActionResult Book(string slug)
        {
            var project = _db.Projects
                .FirstOrDefault(p => p.Slug == slug && p.Category == "book" && p.IsPublished);

            if (project == null) return NotFound();

            return View(ProjectDisplayHelper.BuildDetailViewModel(project));
        }

        // GET /websites
        public IActionResult Websites()
        {
            var websites = _db.Projects
                .Where(p => p.Category == "website" && p.IsPublished)
                .OrderBy(p => p.SortOrder)
                .ThenByDescending(p => p.CreatedAt)
                .ToList();

            return View(websites);
        }

        // GET /websites/{slug}
        public IActionResult Website(string slug)
        {
            var project = _db.Projects
                .FirstOrDefault(p => p.Slug == slug && p.Category == "website" && p.IsPublished);

            if (project == null) return NotFound();

            return View(ProjectDisplayHelper.BuildDetailViewModel(project));
        }

        // GET /misc
        public IActionResult Miscellaneous()
        {
            var projects = _db.Projects
                .Where(p => p.Category == "misc" && p.IsPublished)
                .OrderBy(p => p.SortOrder)
                .ThenByDescending(p => p.CreatedAt)
                .ToList();

            return View(projects);
        }

        // GET /misc/{slug}
        public IActionResult MiscProject(string slug)
        {
            var project = _db.Projects
                .FirstOrDefault(p => p.Slug == slug && p.Category == "misc" && p.IsPublished);

            if (project == null) return NotFound();

            return View(ProjectDisplayHelper.BuildDetailViewModel(project));
        }
    }
}
