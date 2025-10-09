using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeronikaPortfolio.Data;
using WeronikaPortfolio.Models;

namespace WeronikaPortfolio.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Admin Dashboard
        public async Task<IActionResult> Index()
        {
            var projects = await _context.Projects
                .Include(p => p.Images)
                .ToListAsync();

            var about = await _context.AboutSections.FirstOrDefaultAsync() ?? new AboutSection();

            var viewModel = new AdminViewModel
            {
                Projects = projects,
                About = about
            };

            return View(viewModel);
        }

        [HttpPost]
public async Task<IActionResult> AddProject(Project project, List<IFormFile> Images)
{
    if (!ModelState.IsValid)
        return RedirectToAction("Index");

    // Create uploads folder if it doesn't exist
    var uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
    if (!Directory.Exists(uploadDir))
        Directory.CreateDirectory(uploadDir);

    // Initialize image list
    project.Images = new List<ProjectImage>();

    // Save uploaded images
    if (Images != null && Images.Count > 0)
    {
        foreach (var image in Images)
        {
            var extension = Path.GetExtension(image.FileName);
            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadDir, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            project.Images.Add(new ProjectImage
            {
                ImagePath = "/uploads/" + fileName
            });
        }
    }

    // Save project to database
    _context.Projects.Add(project);
    await _context.SaveChangesAsync();

    return RedirectToAction("Index");
}


        // Delete Project
        [HttpPost]
        public async Task<IActionResult> DeleteProject(int id)
        {
            var project = await _context.Projects
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (project != null)
            {
                // Remove associated images
                _context.ProjectImages.RemoveRange(project.Images);
                _context.Projects.Remove(project);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        // Update About Section
        [HttpPost]
        public async Task<IActionResult> UpdateAbout(AboutSection about)
        {
            var existing = await _context.AboutSections.FirstOrDefaultAsync();
            if (existing != null)
            {
                existing.BioText = about.BioText;
                existing.ProfileImagePath = about.ProfileImagePath;
                _context.AboutSections.Update(existing);
            }
            else
            {
                _context.AboutSections.Add(about);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // Add Project Images
        [HttpPost]
        public async Task<IActionResult> AddProjectImages(int projectId, List<string> imagePaths)
        {
            var project = await _context.Projects
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == projectId);

            if (project != null)
            {
                foreach (var path in imagePaths)
                {
                    project.Images.Add(new ProjectImage { ImagePath = path });
                }

                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }
    }
}