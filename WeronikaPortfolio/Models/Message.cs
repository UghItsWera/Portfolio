using System;
using System.ComponentModel.DataAnnotations;

namespace WeronikaPortfolio.Models
{
    public class Message
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(200)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        public DateTime DateSent { get; set; } = DateTime.Now;
    }
}