using System.ComponentModel.DataAnnotations;

namespace PortfolioCMS.Models
{
    public class Project
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        public string Slug { get; set; } = string.Empty;

        [Required]
        public string Category { get; set; } = string.Empty; // "game" | "website" | "book" | "extra"

        public string? Summary { get; set; }

        public string? Body { get; set; }

        public string? CoverImage { get; set; }

        // JSON arrays stored as strings
        public string? Images { get; set; }   // [{ "url": "", "alt": "" }]
        public string? Buttons { get; set; }  // [{ "label": "", "url": "", "style": "" }]
        public string? Tags { get; set; }     // ["tag1", "tag2"]
        public string? Metadata { get; set; } // flexible per category e.g. { "platform": "PC", "rating": "4.5" }

        public bool IsPublished { get; set; } = false;

        public int SortOrder { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}