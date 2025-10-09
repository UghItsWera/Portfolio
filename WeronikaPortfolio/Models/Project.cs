namespace WeronikaPortfolio.Models;

public class Project
{
    public int Id { get; set; }
    public string Title { get; set; }          // Project name
    public string Description { get; set; }    // Short blurb
    public string Link { get; set; }           // External link (GitHub, etc.)
    public string VideoUrl { get; set; }       // Embedded video link (e.g. YouTube)
    public List<ProjectImage> Images { get; set; } = new(); // For slideshow
}