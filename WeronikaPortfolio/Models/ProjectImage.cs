public class ProjectImage
{
    public int Id { get; set; }
    public string ImagePath { get; set; } // File path to uploaded image
    public int ProjectId { get; set; }
    public Project Project { get; set; }
}