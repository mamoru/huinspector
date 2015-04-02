using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using huinspector.Models;
using System.IO;

namespace huinspector.Catalog
{
    public static class EvalCatalog
    {

        public static IList<Evaluation> GetEval(int? id)
        {
            using (var db = new HUInspectorEntities())
            {
                return (from s in db.Evaluation where s.ExamId == id select s).Include(i => i.Exam).Include(i => i.User).ToList();

            }
        }
        public static IList<Evaluation> GetAll()
        {
            using (var db = new HUInspectorEntities())
            {
                return db.Evaluation.Include(i => i.ExamId).Include(i => i.User).ToList();

            }
        }
        public static int CheckUsers(int? id, int userType)
        {
            using (var db = new HUInspectorEntities())
            {
                return (from s in db.Evaluation where s.ExamId == id && s.User.UserTypeId == userType select s).Include(i => i.Exam).Include(i => i.User).Count();
            }
        }

        public static byte[] GetFile(int? id)
        {
            using (var db = new HUInspectorEntities())
            {
                return (from s in db.Evaluation where s.Id == id select s.Document).SingleOrDefault();
            }
        }


    }
}