using System;
using System.ComponentModel.DataAnnotations;

namespace WeronikaPortfolio.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime SentAt { get; set; } = DateTime.Now;
    }
}