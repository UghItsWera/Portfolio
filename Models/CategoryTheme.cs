namespace PortfolioCMS.Models
{
    public class CategoryTheme
    {
        public int Id { get; set; }

        public string Category { get; set; } = string.Empty;

        // Light mode
        public string LightBackground { get; set; } = "#E0E4E7";
        public string LightAccent { get; set; } = "#52677F";
        public string LightCard { get; set; } = "#D5DEEA";

        // Dark mode
        public string DarkBackground { get; set; } = "#26313D";
        public string DarkAccent { get; set; } = "#AFC4D8";
        public string DarkCard { get; set; } = "#354452";
    }
}