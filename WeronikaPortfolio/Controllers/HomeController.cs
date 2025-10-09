using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeronikaPortfolio.Data;
using WeronikaPortfolio.Models;

namespace WeronikaPortfolio.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index() => View();

        public IActionResult About() => View();

        public async Task<IActionResult> Projects()
{
    var projects = await _context.Projects
        .Include(p => p.Images)
        .ToListAsync();

    var about = await _context.AboutSections.FirstOrDefaultAsync() ?? new AboutSection();

    var viewModel = new ProjectsViewModel
    {
        Projects = projects,
        About = about
    };

    return View(viewModel);
}

        [HttpPost]
        public async Task<IActionResult> Contact(Message message)
        {
            if (ModelState.IsValid)
            {
                _context.Messages.Add(message);
                await _context.SaveChangesAsync();
                ViewBag.Status = "🌸 Thanks for your message!";
            }
            else
            {
                ViewBag.Status = "Please fill in all fields.";
            }

            return View("Index");
        }
    }
}