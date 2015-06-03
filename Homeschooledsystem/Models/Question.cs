using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Homeschooledsystem.Models
{
    public partial class Question
    {
        public Question()
        {
            this.Answer = new HashSet<Answer>();
        }

        private int _importance = 1;

        public int Importance
        {
            get { return _importance; }
            set
            {
                if (value > 3 || value < 1)
                    throw new Exception("Wrong importance value");
                _importance = value;
            }
        }

        public int QuestionId { get; set; }
        [Display(Name = "Текст")]
        public string Text { get; set; }

        public virtual ICollection<Answer> Answer { get; set; }
        public virtual Test Test { get; set; }
    }
}