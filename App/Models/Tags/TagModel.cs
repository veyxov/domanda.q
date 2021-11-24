using System;
using System.ComponentModel.DataAnnotations;

namespace App.Models 
{
    public class Tag
    {
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }

        public Nullable<Guid> QuestionId { get; set; }
    }
}
