using System.Text.Json;
using PortfolioCMS.Models;

namespace PortfolioCMS.Helpers
{
    public static class ProjectDisplayHelper
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public static T? TryDeserialize<T>(string? json)
        {
            if (string.IsNullOrWhiteSpace(json)) return default;

            try
            {
                return JsonSerializer.Deserialize<T>(json, JsonOptions);
            }
            catch (JsonException)
            {
                return default;
            }
        }

        public static List<string> ParseTags(string? tagsJson)
        {
            if (string.IsNullOrWhiteSpace(tagsJson)) return new();

            var tags = TryDeserialize<List<string>>(tagsJson) ?? new();
            if (tags.Count > 0) return tags;

            var trimmed = tagsJson.Trim();
            if (!trimmed.StartsWith('['))
            {
                return trimmed
                    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .ToList();
            }

            return tags;
        }

        public static ProjectDetailViewModel BuildDetailViewModel(Project project)
        {
            var vm = new ProjectDetailViewModel { Project = project };

            vm.Buttons = TryDeserialize<List<ButtonItem>>(project.Buttons) ?? new();
            vm.Images = TryDeserialize<List<ImageItem>>(project.Images) ?? new();
            vm.Tags = ParseTags(project.Tags);

            if (!string.IsNullOrEmpty(project.Metadata))
            {
                vm.GameMeta = project.Category == "game"
                    ? TryDeserialize<GameMetadata>(project.Metadata)
                    : null;
                vm.BookMeta = project.Category == "book"
                    ? TryDeserialize<BookMetadata>(project.Metadata)
                    : null;
                vm.WebsiteMeta = project.Category == "website"
                    ? TryDeserialize<WebsiteMetadata>(project.Metadata)
                    : null;
            }

            return vm;
        }

        public static string TeaserSubtitle(Project project)
        {
            var tags = ParseTags(project.Tags);
            if (tags.Count > 0) return tags[0];

            return project.Category switch
            {
                "game" => "Game",
                "book" => "Book",
                "website" => "Website",
                _ => "Project"
            };
        }

        public static string CoverOrPlaceholder(Project project, string placeholderPath = "/images/general-img-landscape.png")
        {
            return string.IsNullOrWhiteSpace(project.CoverImage) ? placeholderPath : project.CoverImage;
        }
    }
}
