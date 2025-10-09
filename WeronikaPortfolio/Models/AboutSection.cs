using System;
using System.ComponentModel.DataAnnotations;

namespace WeronikaPortfolio.Models;

public class AboutSection
{
    public int Id { get; set; }
    public string BioText { get; set; }
    public string ProfileImagePath { get; set; }
}