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
    }
}
