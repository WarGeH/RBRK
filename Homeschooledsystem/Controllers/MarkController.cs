using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Homeschooledsystem.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Homeschooledsystem.Controllers
{
    public class MarkController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Marks
        [Authorize(Roles = "student")]
        public ActionResult Student()
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            string user = User.Identity.GetUserId();
            var course = db.Subscription.Include(l => l.Subscriber).Where(l => l.Subscriber.Id == user);
            //var mark = db.Mark.Include(l => l.Student).Where(l => l.Student.Id == user);
            return View(course.ToList());
        }

        public ActionResult Details(int id)
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            string user = User.Identity.GetUserId();
            var mark = db.Mark.Include(l => l.Student).Where(l => l.Student.Id == user && l.Test.Lecture.Course.id == id);
            ViewBag.Tests = mark;
            
            int countmarks = db.Mark.Count();
            var lecture = (db.Lecture.Where(l => l.CourseId == id)).ToList<Lecture>();
            ViewBag.CourseId = lecture[0].Course.id;
            ViewBag.MarkCount = countmarks;
            ViewBag.Lectures = lecture;
            return View(mark.ToList());
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}