using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Models
{
    public class Comment
    {
        [Key]
        public Guid Id { get; set; }

        public string Text { get; set; }

        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        public DateTime CreationDate { get; set; }

        public Guid? QuestionId { get; set; }
        [ForeignKey("QuestionId")]
        public virtual Question Question { get; set; }

        public Guid? AnswerId { get; set; }
        [ForeignKey("AnswerId")]
        public virtual Answer Answer  { get; set; }
    }
}
