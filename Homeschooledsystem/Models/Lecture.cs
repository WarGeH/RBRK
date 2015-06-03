using System.ComponentModel.DataAnnotations;

namespace Homeschooledsystem.Models
{
    public partial class Lecture
    {
        public int Id { get; set; }
        public int CourseId { get; set; }

        [Display(Name = "Название лекции")]
        public string Name { get; set; }

        [Display(Name = "Содержание")]
        public string Text { get; set; }
        public int Number { get; set; }

        public virtual Course Course { get; set; }
    }
}