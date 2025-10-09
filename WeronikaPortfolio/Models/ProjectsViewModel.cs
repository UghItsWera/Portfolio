using System.Collections.Generic;

namespace WeronikaPortfolio.Models
{
    public class ProjectsViewModel
    {
        public List<Project> Projects { get; set; } = new List<Project>();
        public AboutSection About { get; set; } = new AboutSection();
    }
}