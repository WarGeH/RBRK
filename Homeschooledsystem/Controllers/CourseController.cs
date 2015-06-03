using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Homeschooledsystem.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;


namespace Homeschooledsystem.Controllers
{
    public class CourseController : Controller
    {

        private ApplicationDbContext db = new ApplicationDbContext();


        // GET: /Course/
        public ActionResult List()
        {
            var course = db.Courses.Where(l => l.activated == true);
            return View(course.ToList());
        }


        public ActionResult Comment(int courseId)
        {
            ViewBag.CourseId = courseId;
            return View();
        }


        public ActionResult CommentList(int courseId)
        {
            Course course = db.Courses.First(c => c.id == courseId);

            string userId = User.Identity.GetUserId();
            ViewBag.UserId = userId;
            ViewBag.NeededTeacher = false;

            if (userId == course.Author.Id)
            {
                ViewBag.NeededTeacher = true;
            }

            List<Comment> comments = db.Comment.Where(c => c.course.id == courseId && c.previosComment == null).ToList<Comment>();

            List<Comment> teacherComments = db.Comment.Where(c => c.course.id == courseId && c.previosComment != null).ToList<Comment>();

            ViewBag.CommentList = comments;
            ViewBag.TeacherComments = teacherComments;

            return View();
        }


        public ActionResult DeleteComment(int commentId)
        {
            var com = db.Comment.First(c => c.id == commentId);
            int courseId = com.course.id;
            db.Comment.Remove(com);

            List<Comment> tComments = db.Comment.Where(tc => tc.previosComment != null && tc.previosComment.id == commentId).ToList<Comment>();

            if (tComments.Count > 0)
            {
                foreach (var c in tComments)
                {
                    db.Comment.Remove(c);
                }
            }

            db.SaveChanges();
            return Redirect("View/" + courseId);
        }

        [HttpPost]
        // [ValidateAntiForgeryToken]
        public ActionResult AddComment(string commentText, int courseId)
        {
            var fCourse = db.Courses.First(c => c.id == courseId);
            string userId = User.Identity.GetUserId();
            var u = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));

            db.Comment.Add(new Comment { text = commentText, course = fCourse, user = u.FindById(userId), date = DateTime.Now, });
            db.SaveChanges();


            return Redirect("View/" + courseId);
        }

        //[HttpPost]
        // [ValidateAntiForgeryToken]
        public ActionResult AddTeacherComment(string commentText, int courseId, int prevComment)
        {
            var fCourse = db.Courses.First(c => c.id == courseId);
            string userId = User.Identity.GetUserId();
            var u = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            var comment = db.Comment.First(c => c.id == prevComment);
            db.Comment.Add(new Comment { text = commentText, previosComment = comment, course = fCourse, user = u.FindById(userId), date = DateTime.Now, });
            db.SaveChanges();


            return Redirect("View/" + courseId);
        }

        public ActionResult CourseFinish(int id)
        {
            var course = db.Courses.First(c => c.id == id);
            course.isFinished = true;
            db.SaveChanges();
            return RedirectToAction("Teacher", "Course");
            //    return View();
        }

        [Authorize(Roles = "teacher")]
        public ActionResult Teacher()
        {

            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            string user = User.Identity.GetUserId();
            var course = db.Courses.Include(l => l.Author).Where(l => l.Author.Id == user);
            return View(course.ToList());
        }

        public ActionResult Search(Search search)
        {
            ViewBag.Keywords = search.keywords;
            ViewBag.Category = search.category_id;
            ViewBag.SortField = search.sort_field;
            var course = db.Courses.Where(l => l.name.Contains(search.keywords) && (search.category_id == 0 || l.Category.id == search.category_id)).ToList();
            List<Course> sorted = new List<Course>();
            switch (search.sort_field)
            {
                case "name":
                    sorted = course.OrderBy(m => m.name).ToList();
                    break;
                case "category":
                    sorted = course.OrderBy(m => m.Category.Name).ToList();
                    break;
                case "description":
                    sorted = course.OrderBy(m => m.description).ToList();
                    break;
                case "author":
                    sorted = course.OrderBy(m => m.Author.FirstName + m.Author.LastName).ToList();
                    break;
                case "date":
                    sorted = course.OrderBy(m => m.PublishDate).ToList();
                    break;
                default:
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return View(sorted);
        }

        private bool IsNeededTeacher(string courseId)
        {
            var u = User.Identity.GetUserId();
            return u==courseId;
        }


        [Authorize(Roles = "teacher")]
        // GET: /Course/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Course course = db.Courses.Find(id);
            ViewBag.Teacher = IsNeededTeacher(course.Author.Id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }


        [Authorize(Roles = "teacher")]
        public ActionResult Subscribers(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            Course course = db.Courses.Find(id);
            var subscribe = db.Subscription.FirstOrDefault(l => l.Course.id == id);
            //var studentsTemp = db.Users.Where(l => l.Id == subscribe.Subscriber.Id);
            List<ApplicationUser> studentsTemp = db.Users.ToList<ApplicationUser>();
            List<ApplicationUser> students = new List<ApplicationUser>();
            if(subscribe != null)
            foreach(var item in studentsTemp)
            {
                if(subscribe.Subscriber.Id == item.Id)
                {
                    students.Add(item);
                }
            }
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(students);
        }

        [Authorize(Roles = "teacher")]
        public ActionResult Passed(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            var subscribe = db.Subscription.FirstOrDefault(l => l.Course.id == id);
            List<ApplicationUser> passed = new List<ApplicationUser>();
            List<ApplicationUser> studentsTemp = db.Users.ToList<ApplicationUser>();
            List<ApplicationUser> students = new List<ApplicationUser>();
            if (subscribe != null)
            foreach (var item in studentsTemp)
            {
                if (subscribe.Subscriber.Id == item.Id)
                {
                    students.Add(item);
                }
            }
            var tests = db.Test.Where(t => t.Lecture.Course.id == id).ToList();
            bool isFinished = db.Courses.FirstOrDefault(c => c.id == id).isFinished;
            int right = 0;
            if (isFinished)
            {
                foreach (var stud in students)
                {
                    var marks = db.Mark.Where(m => m.Test.Lecture.Course.id == id && m.Student.Id == stud.Id).ToList();
                    right = 0;
                    foreach (var item in marks)
                    {
                        if (item.Value >= 60 && marks.Count == tests.Count)
                        {
                            right++;
                        }
                        if (right == tests.Count && isFinished)
                        {
                            passed.Add(stud);
                        }
                    }
                }
            }
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(passed);
        }

        [HttpPost]
        public RedirectResult Mark(int value, int courseId)
        {
            var marks = db.CourseMark.ToList<CourseMark>();
            List<CourseMark> lcm = new List<CourseMark>();
            foreach (var m in marks)
            {
                if (m.course.id == courseId)
                {
                    lcm.Add(m);
                }
            }
            foreach (var m in lcm)
            {
                if (m.user.UserName == User.Identity.Name)
                {
                    m.mark = value;
                    db.SaveChanges();
                    return Redirect("/Course/View/" + courseId);
                }
            }
            Course course = db.Courses.Find(courseId);
            CourseMark mark = new CourseMark();
            mark.course = course;
            mark.mark = value;
            List<ApplicationUser> users = db.Users.ToList<ApplicationUser>();
            foreach (ApplicationUser i in users)
            {
                if (i.UserName == User.Identity.Name)
                {
                    mark.user = i;
                    break;
                }
            }
            db.CourseMark.Add(mark);
            db.SaveChanges();
            return Redirect("/Course/View/" + courseId);
        }
        public ActionResult View(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            List<CourseMark> marks = db.CourseMark.ToList<CourseMark>();
            List<CourseMark> courseMarks = new List<CourseMark>();
            int userMark = 0;
            foreach (CourseMark mark in marks)
            {
                if (mark.course.id == id)
                    courseMarks.Add(mark);
            }
            if (courseMarks.Count == 0)
            {
                ViewBag.mark = "Нет оценок";
                ViewBag.userMark = 0;
            }
            else
            {
                int value = 0;
                foreach (CourseMark mark in courseMarks)
                {
                    if (mark.user.UserName == User.Identity.Name)
                    {
                        userMark = mark.mark;
                    }
                    value += mark.mark;
                }
                ViewBag.mark = Math.Round((double)value / courseMarks.Count, 2);
                ViewBag.userMark = userMark;
            }

            ViewBag.userId = User.Identity.GetUserId();
            return View(course);
        }

        // GET: /Course/Create

        [Authorize(Roles = "teacher")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Course/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.

        [Authorize(Roles = "teacher")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AddCourseModel courseview)
        {

            if (ModelState.IsValid)
            {

                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
                var course = new Course() { name = courseview.name, description = courseview.description, Category = db.Categories.Find(courseview.category_id), sertificate = courseview.sertificate };
                course.PublishDate = DateTime.Now;
                course.activated = false;
                course.Author = userManager.FindById(User.Identity.GetUserId());
                db.Courses.Add(course);
                db.SaveChanges();
                return RedirectToAction("Teacher");
            }

            return View(courseview);
        }

        // GET: /Course/Edit/5

        [Authorize(Roles = "teacher")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(new AddCourseModel(course));
        }

        // POST: /Course/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.

        [Authorize(Roles = "teacher")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(AddCourseModel courseview)
        {
            if (ModelState.IsValid)
            {
                var course = db.Courses.Find(courseview.id);
                course.name = courseview.name;
                course.description = courseview.description;
                course.Category = db.Categories.Find(courseview.category_id);
                course.sertificate = courseview.sertificate;

                db.Entry(course).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Teacher");
            }
            return View(courseview);
        }

        // GET: /Course/Delete/5

        [Authorize(Roles = "teacher, admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        // POST: /Course/Delete/5

        [Authorize(Roles = "teacher, admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Course course = db.Courses.Find(id);
            db.Courses.Remove(course);
            db.SaveChanges();
            if (User.IsInRole("admin"))
            {

                return RedirectToAction("List");
            }
            else
            {

                return RedirectToAction("Teacher");
            }
        }

        public ActionResult Requests()
        {
            var course = db.Courses.Where(l => l.activated == false);
            return View(course.ToList());
        }

        public ActionResult ChangeStatus(int id)
        {
            var course = db.Courses.Find(id);
            course.activated = !course.activated;
            db.Entry(course).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Requests");
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
