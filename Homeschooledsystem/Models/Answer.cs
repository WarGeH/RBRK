using System;
using System.ComponentModel.DataAnnotations;

namespace Homeschooledsystem.Models
{
    public partial class Answer
    {

        public int Id { get; set; }
        public Nullable<int> QuestionId { get; set; }
        [Display(Name = "Текст")]
        public string Text { get; set; }
        public bool IsTrue { get; set; }

        public virtual Question Question { get; set; }
    }
}