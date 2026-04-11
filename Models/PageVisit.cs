namespace PortfolioCMS.Models
{
    public class PageVisit
    {
        public int Id { get; set; }
        public string Path { get; set; } = string.Empty;
        public DateTime VisitedAt { get; set; } = DateTime.UtcNow;
        public string? UserAgent { get; set; }
        public string? IpAddress { get; set; }
    }
}