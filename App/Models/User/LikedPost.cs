using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Models
{
    public class LikedPost
    {
        public Guid Id { get; set; }

        // Post that is liked
        public Guid PostId { get; set; }
        public virtual Post Post { get; set; }

        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}
