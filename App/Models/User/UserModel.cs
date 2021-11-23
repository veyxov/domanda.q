using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace App.Models
{
    public class User : IdentityUser
    {

        [Display(Name="Profile picture")]
        public string ProfilePicPath { get; set; }

        public virtual ICollection<Question> Questions { get; set; }
        public virtual ICollection<Answer> Answers { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<LikedPost> LikedPosts { get; set; }
    }
}
