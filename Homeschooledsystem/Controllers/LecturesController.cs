using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Homeschooledsystem.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Homeschooledsystem.Controllers
{
    public class LecturesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private static int courseId;

        // GET: Lectures
        public ActionResult Index([Bind(Include = "id")] Course course)
        {
            courseId = course.id;
            ViewBag.CourseId = course.id;
            var lecture = db.Lecture.Include(l => l.Course).Where(l => l.Course.id == courseId);
            return PartialView(lecture.ToList());
        }

        public ActionResult EditList([Bind(Include = "id")] Course course)
        {
            courseId = course.id;
            var lecture = db.Lecture.Include(l => l.Course).Where(l => l.Course.id == courseId);
            return PartialView(lecture.ToList());
        }


        private bool IsNeededTeacher(string courseId)
        {
            var u = User.Identity.GetUserId();
            return u == courseId;
        }


        public ActionResult View(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Lecture lecture = db.Lecture.Find(id);
            if (lecture == null)
            {
                return HttpNotFound();
            }
            return View(lecture);
        }

        // GET: Lectures/Create
        public ActionResult Create()
        {
            ViewBag.Teacher = false;
            if (courseId > 0)
            {
                var c = db.Courses.Find(courseId);
                ViewBag.Teacher = IsNeededTeacher(c.Author.Id);
            }
            return View();
        }

		[HttpPost]
        public string Upload(IEnumerable<HttpPostedFileBase> fileUpload)
        {
            foreach (var file in fileUpload)
            {
                if (file == null) continue;
                string path = AppDomain.CurrentDomain.BaseDirectory + "resources/Media/";
                string filename = Path.GetFileName(file.FileName);
                if (filename != null)
                    try
                    {
                        file.SaveAs(Path.Combine(path, filename));
                    }
                    catch
                    {
                        return "File didn't loaded on server!";
                    }
            }

            return "";
        }
		
        // POST: Lectures/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create([Bind(Include = "Id,CourseId,Name,Text")] Lecture lecture)
        {
            if (ModelState.IsValid)
            {
                lecture.Course = db.Courses.Find(courseId);
                db.Lecture.Add(lecture);
                db.SaveChanges();
                int c = courseId;
                courseId = 0;
                return Redirect("/Course/Details/" + c);
            }

            //ViewBag.CourseId = new SelectList(db.Courses, "id", "name", lecture.CourseId);
            return View(lecture);
        }

        // GET: Lectures/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Lecture lecture = db.Lecture.Find(id);
            ViewBag.Teacher = IsNeededTeacher(lecture.Course.Author.Id);
            if (lecture == null)
            {
                return HttpNotFound();
            }
            ViewBag.CourseId = new SelectList(db.Courses, "id", "name", lecture.CourseId);
            return View(lecture);
        }

        // POST: Lectures/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit([Bind(Include = "Id,CourseId,Name,Text")] Lecture lecture)
        {
            if (ModelState.IsValid)
            {
                lecture.Course = db.Courses.Find(courseId);
                lecture.CourseId = courseId;
                db.Entry(lecture).State = EntityState.Modified;
                db.SaveChanges();
                return Redirect("/Course/Details/" + courseId);
            }
            ViewBag.CourseId = new SelectList(db.Courses, "id", "name", lecture.CourseId);
            return View(lecture);
        }

        // GET: Lectures/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Lecture lecture = db.Lecture.Find(id);
            ViewBag.Teacher = IsNeededTeacher(lecture.Course.Author.Id);
            if (lecture == null)
            {
                return HttpNotFound();
            }
            return View(lecture);
        }

        // POST: Lectures/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Lecture lecture = db.Lecture.Find(id);
            db.Lecture.Remove(lecture);
            db.SaveChanges();
            int c = courseId;
            courseId = 0;
            return Redirect("/Course/Details/" + c);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult RedirectToCourseView()
        {
            return Redirect("/Course/Details/" + courseId);
        }
    }
}
