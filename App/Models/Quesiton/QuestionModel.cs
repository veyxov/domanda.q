using System.Collections.Generic;

namespace App.Models
{
    public class Question : Post
    {
        public virtual ICollection<Answer> Answers { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<Tag> Tags { get; set; }

        public virtual bool IsSolved { get; set; } = false;
        public override string Type { get; set; } = "Question";
    }
}
