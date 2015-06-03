using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Homeschooledsystem.Logic;
using Homeschooledsystem.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Homeschooledsystem.Controllers
{

    namespace CourseManagementSystem.Controllers
    {
        public class TestingController : Controller
        {
            private readonly ApplicationDbContext db = new ApplicationDbContext();

            public static List<string> answerList;
            public static int testId;


            // GET: Testing
            public ActionResult Question(int lectId)
            {

                if (!(db.Question.Count() > 0) && !(db.Test.Count() > 0))
                {
                    return View();
                }
                var t = db.Test.Where(l => l.Lecture.Id == lectId).ToList();
                ViewBag.TestId = t[0].Id;

                testId = t[0].Id;

                var questions = db.Question.Where(l => l.Test.Id == testId).Include(q => q.Answer);
                ViewBag.Questions = questions;
                answerList = new List<string>();
                ViewBag.AnswerList = answerList;
                return View();
            }

            [HttpPost]
            [Authorize(Roles = "student")]
            public ActionResult Question()
            {
                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
                string user = User.Identity.GetUserId();
                var count = Request.Form.Count;
                string[][] results = new string[count][];
                var tests = db.Test.Where(t => t.Id == testId).ToList();
                var t1 = tests[0];
                var lectures = db.Lecture.Where(l => l.Id == t1.Lecture.Id).ToList();
                for (int i = 0; i < count; i++)
                {
                    results[i] = Request.Form.GetValues(i);
                }
                int test = testId;
                var testAnswers =
                    db.Answer.Where(l => l.Question.Test.Id == test);
                var rightAnswers = testAnswers.Where(a => a.IsTrue).ToList();
                var questions = db.Question.Where(q => q.Test.Id == test).ToList();
                int qwestionPower = MarkCount.CountingQwestionPower(testId);
                int right = MarkCount.CountingRightAnswers(results, questions, testId);
              

                var l1 = lectures[0];
                var sub = db.Subscription.First(u => u.Subscriber.Id == user && u.Course.id == l1.Course.id);
                int mark = 0;
                if (right == qwestionPower)
                {
                    mark = 100;
                }
                else
                {
                    mark = 100 / qwestionPower * right;

                }
                var idCourse = db.Courses.First(c => c.id == l1.Course.id).id;
                
                db.Mark.Add(new Mark() { Value = mark, Student = userManager.FindById(user), Test = db.Test.First(t => t.Id == testId) });
                var isMarkExsist = db.Mark.Where(d => d.Student.Id == user && d.Test.Id == testId).ToList();
                if (isMarkExsist.Count == 1)
                {
                    return RedirectToAction("Details", "Mark", new { id = idCourse });
                }
                else
                {
                    db.SaveChanges();
                }
                return RedirectToAction("Details", "Mark", new { id = idCourse });
            }


            public ActionResult Retake(int lectId)
            {
                if (!MarkCount.IsTestPassed())
                {
                    return View();
                }
                int testId;
                ViewBag.Questions = MarkCount.Retake(lectId, out testId);
                ViewBag.TestId = testId;
                return View();
            }


            [HttpPost]
            [Authorize(Roles = "student")]
            public ActionResult Retake()
            {
                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
                string user = User.Identity.GetUserId();
                var count = Request.Form.Count;
                string[][] results = MarkCount.GetResults(Request);

                string userId = userManager.FindById(user).Id;
                
                MarkCount.SaveMark(results, userId, testId);

                return RedirectToAction("Details", "Mark",
                    new
                    {
                        id = MarkCount.GetCourseIdByTestId(testId)
                    });
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
}