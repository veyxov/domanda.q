using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace App.Models
{
    public class User : IdentityUser
    {

        public virtual ICollection<Question> Questions { get; set; }
    }
}
