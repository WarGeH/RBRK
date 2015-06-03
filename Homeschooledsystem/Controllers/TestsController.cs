using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Drawing;
using Homeschooledsystem.Models;

namespace Homeschooledsystem.Controllers
{
    public class TestsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Tests
        public ActionResult Index()
        {
            return View(db.Test.ToList());
        }

        public ActionResult QuestionsList(int lectId)
        {
            if (!(db.Question.Count() > 0) && !(db.Test.Count() > 0))
            {
                return View();
            }
            ViewBag.LectId = lectId;

            var lect = (db.Lecture.Where(l => l.Id == lectId)).ToList();
            int courseId = lect[0].CourseId;
            var c = (db.Courses.Where(l => l.id == courseId)).ToList<Course>();
            ViewBag.CourseId = c[0].id;

            var t = db.Test.Where(l => l.Lecture.Id == lectId).ToList();
            ViewBag.TestId = t[0].Id;

            int tk = t[0].Id;

            var questions = (db.Question.Where(l => l.Test.Id == tk)).ToList<Question>();
            ViewBag.Questions = questions;

            int answersCount = 4;
            List<Answer> answers = new List<Answer>();

            for (int i = 0; i < questions.Count; i++)
            {
                for (int j = 0; j < answersCount; j++)
                {
                    answers.Add(questions[i].Answer.ElementAt(j));
                }
            }
            ViewBag.Answers = answers;

            return View();
        }

        // GET: Tests/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Test test = db.Test.Find(id);
            if (test == null)
            {
                return HttpNotFound();
            }
            return View(test);
        }
        public ActionResult EditList(int courseId)
        {
            List<Test> tests = new List<Test>();
            List<Lecture> c = new List<Lecture>();
            ViewBag.CourseId = courseId;
            foreach (var t in db.Test.ToList())
            {
                if (t.Lecture.CourseId == courseId)
                {
                    tests.Add(t);
                    c.Add(t.Lecture);
                }
            }

            ViewBag.Lectures = c;

            return View(tests);
        }
        // GET: Tests/CreateTest
        public ActionResult CreateTest(int id)
        {
            //TempData["dd"] = 2;
            
            List<Lecture> q = db.Lecture.ToList();
            List<SelectListItem> lects = new List<SelectListItem>();

            foreach (var i in q)
            {
                if (i.CourseId == id)
                    lects.Add(new SelectListItem { Text = i.Name });
            }

            ViewBag.Lectures = lects;
            return View();
        }



        // POST: Tests/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateTest([Bind(Include = "Id")] Test test, string lectureName, int id)
        {
            if (ModelState.IsValid)
            {
                List<Lecture> q = db.Lecture.ToList();
                Lecture neededLect = new Lecture();

                foreach (var i in q)
                {
                    if (i.CourseId == id && i.Name == lectureName)
                    {
                        foreach (var c in db.Test.ToList())
                        {
                            if (c.Lecture.Id == i.Id)
                            {
                                return RedirectToAction("CreateTest", "Tests", new Course { id = id });
                            }
                        }
                        test.Lecture = i;
                        test.LastLectureId = i.Id;
                    }
                }

                db.Test.Add(test);
                db.SaveChanges();
                return RedirectToAction("Details", "Course", new Course { id = id });
            }

            return View(test);
        }

        // GET: Tests/Create
        public ActionResult Create()
        {

            return View();
        }

        // POST: Tests/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,LastLectureId,Number")] Test test)
        {
            if (ModelState.IsValid)
            {
                db.Test.Add(test);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(test);
        }

        // GET: Tests/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Test test = db.Test.Find(id);

            if (test == null)
            {
                return HttpNotFound();
            }
            return View(test);
        }

        // POST: Tests/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,LastLectureId,Number")] Test test)
        {
            if (ModelState.IsValid)
            {
                db.Entry(test).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(test);
        }

        // GET: Tests/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }


            Test test = db.Test.Find(id);
            if (test == null)
            {
                return HttpNotFound();
            }
            return View(test);
        }

        // POST: Tests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, int courseId)
        {
            Test test = db.Test.Find(id);
            Question[] q = db.Question.ToArray<Question>();
            for (int i = 0; i < q.Count(); i++)
            {
                if (q[i].Test.Id == id)
                {
                    deleteAnswers(q[i].QuestionId, q[i]);
                }
            }
            db.SaveChanges();
            db.Test.Remove(test);
            db.SaveChanges();
            // return RedirectToAction("Index");

            //var l = db.Test.Find(id).Lecture.CourseId;
            //int k = l;
            return RedirectToAction("Details", "Course", new { id = courseId });
        }

        private void deleteAnswers(int id, Question question)
        {
            Answer[] a = db.Answer.ToArray<Answer>();
            for (int i = 0; i < a.Count(); i++)
            {
                if (a[i].QuestionId == id)
                {
                    db.Answer.Remove(a[i]);
                }
            }
            db.SaveChanges();
            db.Question.Remove(question);
            db.SaveChanges();
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
