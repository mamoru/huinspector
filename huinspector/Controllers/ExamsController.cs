using huinspector.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;


namespace huinspector.Controllers
{
    public class ExamsController : Controller
    {
        private HUInspectorEntities db = new HUInspectorEntities();

      private string CurrentUserName
        {
            get
            {
                string currentUserName = (string)Session["CurrentLoggedInUserName"];

                if (currentUserName == null)
                    return string.Empty;
                else
                    return currentUserName;
            }
        }

        private int CurrentUserID
        {
            get
            {
                int? currentUserId = (int?)Session["CurrentLoggedInUserID"];

                if (currentUserId == null)
                    return 0;
                else
                    return Convert.ToInt32(currentUserId);
            }
        }

        // GET: Exams
        public ActionResult Index()
        {
            var exam = db.Exam
                .Include(e => e.User.ExamSubscription)
                .Include(e => e.Quarter)
                .Include(e => e.User)
                .Include(e => e.ExamClassroom);

            ViewData["CurrentUserID"] = CurrentUserID;
            return View(exam.ToList());
        }      


        public ActionResult SubscribeForExam(int? id, int? classroomId, int? week)
        {
            
            int parsedExamId;

            if (id == null)
                parsedExamId = 0;
            else
                parsedExamId = Convert.ToInt32(id);

            ExamSubscription newExamSubscription = new ExamSubscription() { ExamId = parsedExamId, UserId = CurrentUserID, ExamClassroomId = classroomId, Week = Convert.ToInt32(week) };

            HUInspectorEntities context = new HUInspectorEntities();

            context.ExamSubscription.Add(newExamSubscription);
            context.SaveChanges();

            
            Response.Redirect("/Exams");
            return View();
        }

        // GET: Exams/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Exam exam = db.Exam.Find(id);
            if (exam == null)
            {
                return HttpNotFound();
            }
            return View(exam);
        }

        // GET: Exams/Create
        public ActionResult Create()
        {
            ViewBag.Creator = new SelectList(db.User, "Id", "FirstName");
            ViewBag.QuarterId = new SelectList(db.Quarter, "Id", "QuarterName");
            ViewBag.Surveillant = new SelectList(db.User, "Id", "FirstName");
            return View();
        }

        // POST: Exams/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Creator,Speciality,ComputerNeeded,Surveillant,ExpectedStudents,ResultIsDecimal,QuarterId,MinutesDuration")] Exam exam)
        {
            if (ModelState.IsValid)
            {
                db.Exam.Add(exam);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Creator = new SelectList(db.User, "Id", "FirstName", exam.Creator);
            ViewBag.QuarterId = new SelectList(db.Quarter, "Id", "QuarterName", exam.QuarterId);
            ViewBag.Surveillant = new SelectList(db.User, "Id", "FirstName", exam.Surveillant);
            return View(exam);
        }

        // GET: Exams/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Exam exam = db.Exam.Find(id);
            if (exam == null)
            {
                return HttpNotFound();
            }
            ViewBag.Creator = new SelectList(db.User, "Id", "FirstName", exam.Creator);
            ViewBag.QuarterId = new SelectList(db.Quarter, "Id", "QuarterName", exam.QuarterId);
            ViewBag.Surveillant = new SelectList(db.User, "Id", "FirstName", exam.Surveillant);
            return View(exam);
        }

        // POST: Exams/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Creator,Speciality,ComputerNeeded,Surveillant,ExpectedStudents,ResultIsDecimal,QuarterId,MinutesDuration")] Exam exam)
        {
            if (ModelState.IsValid)
            {
                db.Entry(exam).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Creator = new SelectList(db.User, "Id", "FirstName", exam.Creator);
            ViewBag.QuarterId = new SelectList(db.Quarter, "Id", "QuarterName", exam.QuarterId);
            ViewBag.Surveillant = new SelectList(db.User, "Id", "FirstName", exam.Surveillant);
            return View(exam);
        }

        // GET: Exams/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Exam exam = db.Exam.Find(id);
            if (exam == null)
            {
                return HttpNotFound();
            }
            return View(exam);
        }

        // POST: Exams/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Exam exam = db.Exam.Find(id);
            db.Exam.Remove(exam);
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
