using System.Collections.Generic;

namespace App.Models
{
    public class Question : Post
    {
        public virtual ICollection<Answer> Answers { get; set; }
    }
}
