using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        public Question()
        {
            CreationDate = DateTime.UtcNow;
        }
    }
}
