using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace App.Models
{
    public class Question
    {
        [Key]
        public Guid Id { get; set; }

        public int Likes {get; set; } = 0;
        public DateTime CreationDate { get; set; }
        public string Heading { get; set; }
        public string Text { get; set; }

        public IEnumerable<Answer> Answers { get; set; }

        public User User { get; set; }
        public string UserId { get; set; }

        public Question()
        {
            CreationDate = DateTime.UtcNow;
        }
    }
}
