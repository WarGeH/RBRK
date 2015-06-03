using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Homeschooledsystem.Models
{
    public partial class Test
    {

        public int Id { get; set; }
        public Test()
        {
            this.Question = new HashSet<Question>();
        }

        [Display(Name = "Имя лекции")]
        public int LastLectureId { get; set; }
        [Display(Name = "Номер")]
        public int Number { get; set; }

        public virtual Lecture Lecture { get; set; }
        public virtual ICollection<Question> Question { get; set; }
    }
}