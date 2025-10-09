using Microsoft.AspNetCore.Mvc;
using WeronikaPortfolio.Data;
using WeronikaPortfolio.Models;
using Microsoft.EntityFrameworkCore;

public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public AdminController(ApplicationDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    public async Task<IActionResult> Index()
    {
        var projects = await _context.Projects.Include(p => p.Images).ToListAsync();
        var about = await _context.AboutSections.FirstOrDefaultAsync();
        return View((projects, about));
    }

    [HttpPost]
    public async Task<IActionResult> AddProject(Project project, List<IFormFile> images)
    {
        if (ModelState.IsValid)
        {
            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            foreach (var image in images)
            {
                string path = Path.Combine(_environment.WebRootPath, "uploads", image.FileName);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }
                _context.ProjectImages.Add(new ProjectImage
                {
                    ImagePath = "/uploads/" + image.FileName,
                    ProjectId = project.Id
                });
            }

            await _context.SaveChangesAsync();
        }
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> UpdateAbout(AboutSection about, IFormFile? profileImage)
    {
        var existing = await _context.AboutSections.FirstOrDefaultAsync();
        if (existing != null)
        {
            existing.BioText = about.BioText;
            if (profileImage != null)
            {
                string path = Path.Combine(_environment.WebRootPath, "uploads", profileImage.FileName);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await profileImage.CopyToAsync(stream);
                }
                existing.ProfileImagePath = "/uploads/" + profileImage.FileName;
            }
        }
        else
        {
            if (profileImage != null)
            {
                string path = Path.Combine(_environment.WebRootPath, "uploads", profileImage.FileName);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await profileImage.CopyToAsync(stream);
                }
                about.ProfileImagePath = "/uploads/" + profileImage.FileName;
            }
            _context.AboutSections.Add(about);
        }
        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }
}