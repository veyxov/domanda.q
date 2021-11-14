using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Models
{
    public class Answer
    {
        [Key]
        public Guid Id { get; set; }

        public int Likes {get; set; } = 0;
        public DateTime CreationDate { get; set; }
        public string Heading { get; set; }
        public string Text { get; set; }

        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        public Guid QuestionId { get; set; }

        public Answer()
        {
            CreationDate = DateTime.UtcNow;
        }

    }
}
