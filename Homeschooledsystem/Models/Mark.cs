using System.ComponentModel.DataAnnotations;

namespace Homeschooledsystem.Models
{
    public class Mark
    {
        public int id { get; set; }

        public virtual ApplicationUser Student { get; set; }

        public virtual Test Test { get; set; }

        [Display(Name = "Оценка")]
        public int Value { get; set; }
    }
}