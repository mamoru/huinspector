using huinspector.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace huinspector.Catalogs
{
    public static class ExamCatalog
    {

        public static IList<Exam> GetExams()
        {
            using (var db = new HUInspectorEntities())
            {
                var exams = (from e in db.Exam.Include(p => p.Quarter).Include(p => p.User) select e).ToList();

                return exams;
            }
        }

        public static IList<Exam> GetExamsToPlan()
        {
            using (var db = new HUInspectorEntities())
            {
                var exams = (from e in db.Exam.Include(p => p.Quarter).Include(p => p.User) where e.ExamClassroom.Count == 0 select e).ToList();

                return exams;
            }
        }

        public static IList<Exam> GetExamsPlanned()
        {
            using (var db = new HUInspectorEntities())
            {
                var exams = (from e in db.Exam.Include(p => p.Quarter).Include(p => p.User) where e.ExamClassroom.Count(p => p.DateTime >= DateTime.Now) != 0 select e).ToList();

                return exams;
            }
        }

        public static IList<Exam> GetExamsNotRated()
        {
            using (var db = new HUInspectorEntities())
            {
                var exams = (from e in db.Exam.Include(p => p.Quarter).Include(p => p.User) where e.ExamClassroom.Count(p => p.DateTime <= DateTime.Now) != 0 && e.ExamSubscription.Count(p => p.IsNA.HasValue) == 0 select e);

                return exams.ToList();
            }
        }

        public static IList<Exam> GetExamsRated()
        {
            using (var db = new HUInspectorEntities())
            {
                var exams = (from e in db.Exam.Include(p => p.Quarter).Include(p => p.User) 
                             where (e.ExamSubscription.Count(p => p.Result.HasValue) != 0 )
                                && e.IsHandled == false
                             select e);

                return exams.ToList();
            }
        }

        public static Exam GetExam(int examId)
        {
            using (var db = new HUInspectorEntities())
            {
                return (from e in db.Exam.Include(p => p.Quarter).Include(p => p.User)
                       where e.Id == examId
                       select e).FirstOrDefault();

            }
        }


        public static void SetHandled(int examId)
        {
            using (var db = new HUInspectorEntities())
            {
                var exam = (from e in db.Exam
                           where e.Id == examId
                           select e).FirstOrDefault();

                exam.IsHandled = true;

                db.SaveChanges();

            }
        }
    }
}