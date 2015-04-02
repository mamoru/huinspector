using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using huinspector.Models;

namespace huinspector.Catalogs
{
    public class EvaluationCatalog
    {
        public static void SetEvaluation(Evaluation Eval)
        {
            using (var db = new HUInspectorEntities())
            {
                db.Evaluation.Add(Eval);
                db.SaveChanges();
            }
        }

        public static Evaluation GetEvaluation(int evalId)
        {
            using (var db = new HUInspectorEntities())
            {
                return (from e in db.Evaluation.Include(p => p.Exam).Include(p => p.User).Include(p => p.User.UserType)
                            where e.Id == evalId
                            select e).FirstOrDefault();
            }
        }



        public static void DeleteEvaluation(Evaluation eval)
        {
            using (var db = new HUInspectorEntities())
            {
                db.Evaluation.Remove(eval);
            }
        }
    }
}