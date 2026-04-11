using System.ComponentModel.DataAnnotations;

namespace PortfolioCMS.Models
{
    public class Announcement
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        public string? Body { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}