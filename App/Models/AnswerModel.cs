using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Models
{
    public class Answer : Post
    {
        // The question ID that the answer belongs
        public Guid QuestionId { get; set; }
        [ForeignKey("QuestionId")]
        public virtual Question Question { get; set; }
        // Asker marked this answer as solution
        public bool IsSolution { get; set; } = false;

        public virtual ICollection<Comment> Comments { get; set; }
    }
}
