using System.Text.Json;
using PortfolioCMS.Models;

namespace PortfolioCMS.Helpers
{
    public static class ProjectDisplayHelper
    {
        public static string TeaserSubtitle(Project p)
        {
            if (!string.IsNullOrEmpty(p.Tags))
            {
                try
                {
                    var tags = JsonSerializer.Deserialize<List<string>>(p.Tags);
                    if (tags is { Count: > 0 })
                        return tags[0];
                }
                catch (JsonException)
                {
                    /* ignore */
                }
            }

            return p.Category switch
            {
                "game" => "Game",
                "book" => "Book",
                "website" => "Website",
                _ => "Project"
            };
        }

        public static string CoverOrPlaceholder(Project p, string placeholderPath = "/images/general-img-landscape.png")
        {
            return string.IsNullOrWhiteSpace(p.CoverImage) ? placeholderPath : p.CoverImage;
        }
    }
}
