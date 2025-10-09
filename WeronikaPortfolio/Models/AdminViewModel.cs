using System.Collections.Generic;

namespace WeronikaPortfolio.Models
{
    public class AdminViewModel
    {
        public List<Project> Projects { get; set; } = new();
        public AboutSection About { get; set; } = new();
    }
}