using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;

namespace Homeschooledsystem.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {

        [Required]
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ConfirmationToken { get; set; }
        public bool IsConfirmed { get; set; }
        public string Skype { get; set; }
    }

    public class ResetToken
    {

        [Key]
        public string Token { get; set; }
        public string UserName { get; set; }

    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        private const string ConnectionString = "Data Source=ANDREW-ПК\\SQLEXPRESS;Initial Catalog=\"CoursesContext (Homeschooledsystem)\";Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False";
        public ApplicationDbContext()
            : base("DefaultConnection")
        {
        }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Comment> Comment { get; set; }

        public DbSet<Course> Courses { get; set; }

        public DbSet<Answer> Answer { get; set; }

        public DbSet<Lecture> Lecture { get; set; }

        public DbSet<Question> Question { get; set; }

        public DbSet<Subscription> Subscription { get; set; }

        public DbSet<Test> Test { get; set; }

        public DbSet<ResetToken> ResetToken { get; set; }

        public DbSet<Mark> Mark { get; set; }

        public DbSet<CourseMark> CourseMark { get; set; }
    }

    public class UniqueEmailAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var userWithTheSameEmail = db.Users.FirstOrDefaultAsync(u => u.Email == (string)value);
            return userWithTheSameEmail == null;
        }
    }
}