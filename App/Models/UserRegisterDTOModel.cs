using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace App.Models
{
    public class UserRegisterDTO
    {
        [Required]
        public string Username { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Compare("Password", ErrorMessage = "Passwords don't match")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Profile image")]
        public IFormFile ProfilePicFile { get; set; }
    }
}
