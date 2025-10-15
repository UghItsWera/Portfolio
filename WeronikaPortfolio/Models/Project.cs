namespace WeronikaPortfolio.Models;

public class Project
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Link { get; set; }  // Optional - can be null
    public string? VideoUrl { get; set; }  // Optional - can be null
    public List<ProjectImage> Images { get; set; } = new();
}