using Microsoft.AspNetCore.Mvc;
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
        public IActionResult Projects() => View();

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