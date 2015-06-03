using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace Homeschooledsystem.Models
{
    public partial class Category
    {
        public int id { get; set; }

        public Category()
        {
            this.Course = new HashSet<Course>();
        }

        [Display(Name = "Категория")]
        public string Name { get; set; }

        public virtual ICollection<Course> Course { get; set; }
    }
       
}