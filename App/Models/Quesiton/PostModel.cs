using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Models
{
    public class Post
    {
        [Key]
        public Guid Id { get; init; }

        public int Likes { get; set; } = 0;
        public DateTime CreationDate { get; set; }

        [Required]
        public string Heading { get; set; }
        [Required]
        public string Text { get; set; }

        // User that posted the post
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}
