using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Homeschooledsystem.Models;

namespace Homeschooledsystem.Controllers
{
    public class QuestionsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private static int testId;
        List<string> a;
       
        // GET: Questions
        public ActionResult Index()
        {
            return View(db.Question.ToList());
        }

        // GET: Questions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Question question = db.Question.Find(id);
            if (question == null)
            {
                return HttpNotFound();
            }
            return View(question);
        }


        // GET: Questions/Create
        public ActionResult Create([Bind(Include = "Id")] Test test)
        {
            testId = test.Id;
            return View();
        }

        // POST: Questions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        // [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Text")] Question question, List<string> names, List<string> impSel, List<string> answers)
        {

            if (ModelState.IsValid)
            {
                question.Test = db.Test.Find(testId);
                question.Importance = Convert.ToInt32(impSel[0].ToString());
                db.Question.Add(question);
                for (int i = 0; i < 4; i++)
                {
                    string istr = i.ToString();
                    db.Answer.Add(new Answer()
                    {
                        IsTrue = answers.Any(t => t == istr),
                        Question = question,
                        QuestionId = question.QuestionId,
                        Text = names[i]
                    });
                }

                db.SaveChanges();

                return RedirectToAction("QuestionsList", "Tests", new { lectId = db.Test.Find(testId).Lecture.Id });
            }

            return View(question);
        }

        // GET: Questions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Question question = db.Question.Find(id);
            if (question == null)
            {
                return HttpNotFound();
            }
            return View(question);
        }

        // POST: Questions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "QuestionId,Text")] Question question)
        {
            if (ModelState.IsValid)
            {
                db.Entry(question).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(question);
        }

        // GET: Questions/Delete/5
        public ActionResult Delete(int? id, int lectId)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBag.LectId = lectId;
            Question question = db.Question.Find(id);
            if (question == null)
            {
                return HttpNotFound();
            }
            return View(question);
        }

        // POST: Questions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, int lectId)
        {
            Question question = db.Question.Find(id);
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
            return RedirectToAction("QuestionsList", "Tests", new { lectId = lectId });
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
