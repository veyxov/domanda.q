using System;
using System.ComponentModel.DataAnnotations;

namespace App.Models
{
    public class Question
    {
        [Key]
        public Guid Id { get; set; }

        public DateTime CreationDate { get; set; }
        public string Heading { get; set; }
        public string Text { get; set; }

        public Question()
        {
            CreationDate = DateTime.UtcNow;
        }
    }
}
