using System;
using System.ComponentModel.DataAnnotations;

namespace Homeschooledsystem.Models
{
    public partial class Course
    {
        
        public int id { get; set; }

        [Display(Name = "Закончен")]
        public bool isFinished { get; set; }    

        [Display(Name = "Название курса")]
        public string name { get; set; }

        [Display(Name = "Описание")]
        public string description { get; set; }

        [Display(Name = "Дата публикации")]
        public System.DateTime PublishDate { get; set; }

        [Display(Name = "Оценка")]
        public Nullable<double> Estimation { get; set; }

        [Display(Name = "Количество оценок")]
        public Nullable<int> EstimationCount { get; set; }

        [Display(Name = "Категория")]
        public virtual Category Category { get; set; }

        [Display(Name = "Автор")]
        public virtual ApplicationUser Author { get; set; }

        [Display(Name = "Статус")]
        public bool activated { get; set; }


        [Display(Name = "Выдача сертификата")]
        public bool sertificate { get; set; }
 


    }
    public partial class AddCourseModel
    {
        public AddCourseModel(Course course) 
        {
            id = course.id;
            name = course.name;
            description = course.description;
            category_id = course.Category.id;
            sertificate = course.sertificate;
        
        }

        public AddCourseModel()
        {

        }


        public int id { get; set; }

        [Display(Name = "Название")]
        public string name { get; set; }

        [Display(Name = "Описание")]
        public string description { get; set; }

        [Display(Name = "Категория")]
        public int category_id { get; set; }


        [Display(Name = "Выдача сертификата")]
        public bool sertificate { get; set; }





    }


}