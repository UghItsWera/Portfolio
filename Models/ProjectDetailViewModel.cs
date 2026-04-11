namespace PortfolioCMS.Models
{
    public class ProjectDetailViewModel
    {
        public Project Project { get; set; } = new();
        public List<ButtonItem> Buttons { get; set; } = new();
        public List<ImageItem> Images { get; set; } = new();
        public List<string> Tags { get; set; } = new();
        public GameMetadata? GameMeta { get; set; }
        public BookMetadata? BookMeta { get; set; }
        public WebsiteMetadata? WebsiteMeta { get; set; }
    }
}