using Microsoft.AspNetCore.Mvc;
using PortfolioCMS.Data;
using PortfolioCMS.Models;
using System.Text.Json;

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

            var vm = BuildDetailViewModel(project);
            return View(vm);
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

            var vm = BuildDetailViewModel(project);
            return View(vm);
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

            var vm = BuildDetailViewModel(project);
            return View(vm);
        }

        // GET /about
        public IActionResult About()
        {
            return View();
        }

        // Shared helper — deserializes all JSON fields into a clean view model
        private ProjectDetailViewModel BuildDetailViewModel(Project project)
        {
            var vm = new ProjectDetailViewModel { Project = project };

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            if (!string.IsNullOrEmpty(project.Buttons))
                vm.Buttons = JsonSerializer.Deserialize<List<ButtonItem>>(project.Buttons, options) ?? new();

            if (!string.IsNullOrEmpty(project.Images))
                vm.Images = JsonSerializer.Deserialize<List<ImageItem>>(project.Images, options) ?? new();

            if (!string.IsNullOrEmpty(project.Tags))
                vm.Tags = JsonSerializer.Deserialize<List<string>>(project.Tags, options) ?? new();

            if (!string.IsNullOrEmpty(project.Metadata))
            {
                if (project.Category == "game")
                    vm.GameMeta = JsonSerializer.Deserialize<GameMetadata>(project.Metadata, options);
                else if (project.Category == "book")
                    vm.BookMeta = JsonSerializer.Deserialize<BookMetadata>(project.Metadata, options);
                else if (project.Category == "website")
                    vm.WebsiteMeta = JsonSerializer.Deserialize<WebsiteMetadata>(project.Metadata, options);
            }

            return vm;
        }
    }
}