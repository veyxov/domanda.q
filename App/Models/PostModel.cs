using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Models
{
    public class Post
    {
        // An ID can not be changed after initialization
        [Key]
        public Guid Id { get; init; }

        public int Likes { get; set; } = 0;
        public DateTime CreationDate { get; set; }

        public string Heading { get; set; }
        public string Text { get; set; }

        // User that posted the post
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}
