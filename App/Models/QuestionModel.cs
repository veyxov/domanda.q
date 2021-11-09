using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Models
{
    public class Question
    {
        [Key]
        public Guid Id { get; set; }

        public string Text { get; set; }
    }
}
