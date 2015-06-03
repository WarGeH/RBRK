using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Homeschooledsystem.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Drawing;
using System;
using System.Text;
using System.IO;
using System.Collections.Generic;

namespace Homeschooledsystem.Controllers
{
    [Authorize(Roles = "student")]
    public class SubscribeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();


        // GET: /Subscribe/
        public ActionResult Index()
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            string user = User.Identity.GetUserId();
            var course = db.Subscription.Include(l => l.Subscriber).Include(l => l.Course).Where(l => l.Subscriber.Id == user);
            //var mark = db.Mark.Include(l => l.Student).Where(l => l.Student.Id == user);
            return View(course.ToList());
        }


        private string RandomForAndrey()
        {
            StringBuilder str = new StringBuilder();
            const int len = 3;
            byte[] bar = new byte[len];
            Random repeatRnd = new Random();
            repeatRnd.NextBytes(bar);

            for (int i = 0; i < len; i++)
            {
                str.Append(bar[i].ToString());
            }

            return str.ToString();
        }
        private string PrintInPicture(Image img, string userFirstName, string userLastName, string courseName)
        {
            FontFamily fontFamily = new FontFamily("Times New Roman");
            Font font = new Font(fontFamily, 30);
            Graphics drawing = Graphics.FromImage(img);
            Brush textBrush = new SolidBrush(Color.Black);
            drawing.DrawString("Серия О", font, textBrush, 35, 224);
            drawing.DrawString(RandomForAndrey(), font, textBrush, 700, 224);
            drawing.DrawString("Подтверждает, что ", font, textBrush, 320, 277);
            drawing.DrawString(userFirstName + " " + userLastName, font, textBrush, 185, 320);
            drawing.DrawString("Прошел курс \"" + courseName + "\"", font, textBrush, 185, 363);
            drawing.DrawString(DateTime.Today.ToString("d"), font, textBrush, 35, 490);
            drawing.Save();

            string direct = userFirstName + "_" + userLastName;
            Directory.CreateDirectory(Server.MapPath("~/resources/cert/" + direct));


            img.Save(Server.MapPath("~/resources/cert/" + direct + "/" + courseName + ".jpeg"));
            textBrush.Dispose();
            drawing.Dispose();

            return "/resources/cert/" + direct + "/" + courseName + ".jpeg";
        }

        public ActionResult Certificate(int courseId)
        {
            string user = User.Identity.GetUserId();
            var u = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            var course = db.Courses.First(c => c.id == courseId);
            string path;

            if (System.IO.File.Exists(Server.MapPath("~/resources/cert/" + u.FindById(user).FirstName + "_" + u.FindById(user).LastName + "/" + course.name + ".jpeg")))
            {
                path = "/resources/cert/" + u.FindById(user).FirstName + "_" + u.FindById(user).LastName + "/" + course.name + ".jpeg";
                ViewBag.Path = path;
                return View();
            }

            try
            {
                Image im = Image.FromFile(Server.MapPath("~/resources/cert/MyCertificate.jpeg"));
                path = PrintInPicture(im, u.FindById(user).FirstName, u.FindById(user).LastName, course.name);
            }
            catch (Exception)
            {
                Image im = Image.FromFile(Server.MapPath("~/resources/cert/MyCertificate.jpeg"));
                im.Save(Server.MapPath("~/resources/cert/MyCertificate1.jpeg"), System.Drawing.Imaging.ImageFormat.Jpeg);
                im = Image.FromFile(Server.MapPath("~/resources/cert/MyCertificate1.jpeg"));
                path = PrintInPicture(im, u.FindById(user).FirstName, u.FindById(user).LastName, course.name);
            }

            ViewBag.Path = path;
            return View();
        }


        // GET: /Subscribe/Details/5
        public ActionResult Details(int? id)
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            string user = User.Identity.GetUserId();

            ViewBag.CourseId = id;

            var marks = db.Mark.Where(m => m.Test.Lecture.Course.id == id && m.Student.Id == user).ToList();
            var tests = db.Test.Where(t => t.Lecture.Course.id == id).ToList();
            bool isFinished = db.Courses.FirstOrDefault(c => c.id == id).isFinished;
            bool isCertificated = db.Courses.FirstOrDefault(s => s.id == id).sertificate;
            bool isFinal = false;
            int right = 0;
            if (isCertificated)
            {
                foreach (var item in marks)
                {
                    if (item.Value >= 60 && marks.Count == tests.Count)
                    {
                        right++;
                    }
                    if (right == tests.Count && isFinished)
                    {

                        isFinal = true;
                    }
                }
            }
            ViewBag.IsFinal = isFinal;
            var lectures = db.Lecture.Where(c => c.CourseId == id).ToList();
            ViewBag.Lectures = lectures;
            return View(lectures.ToList());
        }


        public ActionResult Create(int courseId)
        {
            var subscription = new Subscription();
            subscription.Course = db.Courses.First(c => c.id == courseId);
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            if (ModelState.IsValid)
            {
                subscription.Subscriber = userManager.FindById(User.Identity.GetUserId());
                db.Subscription.Add(subscription);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CourseId = new SelectList(db.Courses, "id", "name", subscription.Course.id);
            return View(subscription);
        }



        // GET: /Subscribe/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Subscription subscription = db.Subscription.Find(id);
            if (subscription == null)
            {
                return HttpNotFound();
            }
            return View(subscription);
        }

        // POST: /Subscribe/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Subscription subscription = db.Subscription.Find(id);
            //Subscription buffSubscr = subscription;

            var user = User.Identity.GetUserId();

            var marks = db.Mark.Where(m => m.Student.Id == user).ToList<Mark>();

            var tests = db.Test.Where(t => t.Lecture.Course.id == subscription.Course.id).ToList<Test>();

            List<Mark> needMarks = new List<Mark>();
            foreach (var mark in marks)
            {
                foreach (var t in tests)
                {
                    if (mark.Test == t)
                    {
                        //needMarks.Add(mark);
                        db.Mark.Remove(mark);
                    }
                }
            }


            db.Subscription.Remove(subscription);
            db.SaveChanges();
            return RedirectToAction("Index");
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
