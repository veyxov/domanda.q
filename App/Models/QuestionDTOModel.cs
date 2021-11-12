using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Models
{
    public class QuestionDTO : Question
    {
        public string CurrentUserName { get; set; }
    }
}
