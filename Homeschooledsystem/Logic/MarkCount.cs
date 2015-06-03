using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Homeschooledsystem.Models;

namespace Homeschooledsystem.Logic
{
    public static class MarkCount 
    {
        public static readonly ApplicationDbContext db = new ApplicationDbContext();

      
        public static bool IsTestPassed()
        {
            return db.Test.Any() && db.Mark.Any();
        }

        public static IEnumerable<Question> Retake(int lectId, out int testId)
        {
            var tests = db.Test.Where(l => l.Lecture.Id == lectId).ToList();
            testId = tests[0].Id;
            int id = testId;
            var questions = db.Question.Where(l => l.Test.Id == id).Include(q => q.Answer);
            return questions;
        }

        public static int CountingQwestionPower(int count)
        {
            int test = count;
            var questions = db.Question.Where(q => q.Test.Id == test).ToList();
            int qwestionPower = 0;
            foreach (var question in questions)
            {
                switch (question.Importance)
                {
                    case 1:
                        qwestionPower += 1;
                        break;
                    case 2:
                        qwestionPower += 2;
                        break;
                    case 3:
                        qwestionPower += 3;
                        break;
                }
            }
            return qwestionPower;
        }

        public static string[][] GetResults(HttpRequestBase request)
        {
            var count = request.Form.Count;
            string[][] results = new string[count][];
            for (int i = 0; i < count; i++)
            {
                results[i] = request.Form.GetValues(i);
            }
            return results;
        }

        public static int CalculateMark(string[][] results, int testId)
        {
            int test = testId;
            var questions = db.Question.Where(q => q.Test.Id == test).ToList();
            int qwestionPower = MarkCount.CountingQwestionPower(testId);
            int right = MarkCount.CountingRightAnswers(results, questions, testId);
            double mark = ((double) right)/qwestionPower*100.0;
            return (int) Math.Round(mark);
        }

        public static void SaveMark(string[][] results, string userId, int testId)
        {
            var idMark = db.Mark.Where(s => s.Student.Id == userId && s.Test.Id == testId).ToList();
            Mark newMark = db.Mark.Find(idMark[0].id);
            newMark.Value = MarkCount.CalculateMark(results, testId);
            newMark.id = idMark[0].id;
            newMark.Test = idMark[0].Test;
            newMark.Student = idMark[0].Student;
            db.Entry(newMark).State = EntityState.Modified;
            db.SaveChanges();
        }

        public static int GetCourseIdByTestId(int testId)
        {
            var t1 = db.Test.First(t => t.Id == testId);
            var l1 = db.Lecture.First(l => l.Id == t1.Lecture.Id);
            var idCourse = db.Courses.First(c => c.id == l1.Course.id).id;
            return idCourse;
        }

        public static int CountingRightAnswers(string[][] results, List<Question> questions, int test)
        {
            questions = db.Question.Where(q => q.Test.Id == test).ToList();
            int right = 0;
            for (int i = 0; i < results.Length; i++)
            {
                bool isTrueUserAnswer = true;
                int questionId = questions[i].QuestionId;
                List<Answer> rightAnswersToQuestion =
                    db.Answer.Where(a => a.QuestionId == questionId && a.IsTrue).ToList();
                if (results[i].Length != rightAnswersToQuestion.Count)
                    isTrueUserAnswer = false;
                else
                    for (int j = 0; j < results[i].Length; j++)
                    {
                        if (results[i][j] != rightAnswersToQuestion[j].Id.ToString())
                            isTrueUserAnswer = false;
                    }
                if (isTrueUserAnswer)
                {
                    var x = questions[i].Importance;
                    if (x > 3 || x < 1)
                        throw new Exception("Wrong Importnse value");
                    right += questions[i].Importance;
                }
            }
            return right;
        }

        public static void Dispose()
        {
            if (db != null)
            {
                db.Dispose();
            }
        }
    }
}