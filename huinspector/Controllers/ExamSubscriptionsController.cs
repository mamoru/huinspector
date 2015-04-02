using huinspector.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;


namespace huinspector.Controllers
{
    public class ExamSubscriptionsController : Controller
    {

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

        private HUInspectorEntities db = new HUInspectorEntities();

        // GET: ExamSubscriptions
        public ActionResult Index()
        {
            var examSubscription = db.ExamSubscription.Distinct().Include(e => e.Exam.Quarter).Include(e => e.User);
            ViewData["CurrentUserName"] = CurrentUserName;
            ViewData["CurrentUserID"] = CurrentUserID;
            return View(examSubscription.ToList());
        }

        public ActionResult SubscribeForExamList()
        {
            var examSubscription = db.ExamSubscription.Include(e => e.Exam.Quarter).Include(e => e.User);
            return View(examSubscription.Distinct().ToList());
        }

        
        public ActionResult SubscribeForExam(int? id)
        {
            int parsedExamId;

            if (id == null)
                parsedExamId = 0;
            else
                parsedExamId = Convert.ToInt32(id);

            ExamSubscription newExamSubscription = new ExamSubscription() { ExamId = parsedExamId, UserId = CurrentUserID };
            
            HUInspectorEntities context = new HUInspectorEntities();
            
            context.ExamSubscription.Add(newExamSubscription);
            context.SaveChanges();
            
            var examSubscription = db.ExamSubscription.Include(e => e.Exam.Quarter).Include(e => e.User);


            return View("SubscribeForExamList", examSubscription.ToList());
        }

        // GET: ExamSubscriptions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ExamSubscription examSubscription = db.ExamSubscription.Find(id);
            if (examSubscription == null)
            {
                return HttpNotFound();
            }
            return View(examSubscription);
        }

        // GET: ExamSubscriptions/Create
        public ActionResult Create()
        {
            ViewBag.ExamId = new SelectList(db.Exam, "Id", "Name");
            return View();
        }

        // POST: ExamSubscriptions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UserId,ExamId,Id,Result,ExamClassroomId,Week,IsNA")] ExamSubscription examSubscription)
        {
            if (ModelState.IsValid)
            {
                db.ExamSubscription.Add(examSubscription);
                db.SaveChanges();
                return RedirectToAction("~/Home/Tentamen");
            }

            ViewBag.ExamId = new SelectList(db.Exam, "Id", "Name", examSubscription.ExamId);
            ViewBag.UserId = new SelectList(db.User, "Id", "FirstName", examSubscription.UserId);
            ViewBag.Week = new SelectList(db.Exam, "Id", "Week", examSubscription.Week);
            return View(examSubscription);
        }

        // GET: ExamSubscriptions/Edit/5
        

        // POST: ExamSubscriptions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UserId,ExamId,Id,Result,ExamClassroomId,Week,IsNA")] ExamSubscription examSubscription)
        {
            if (ModelState.IsValid)
            {
                db.Entry(examSubscription).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ExamId = new SelectList(db.Exam, "Id", "Name", examSubscription.ExamId);
            ViewBag.UserId = new SelectList(db.User, "Id", "FirstName", examSubscription.UserId);
            return View(examSubscription);
        }

        // GET: ExamSubscriptions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ExamSubscription examSubscription = db.ExamSubscription.Find(id);
            if (examSubscription == null)
            {
                return HttpNotFound();
            }
            return View(examSubscription);
        }

        // POST: ExamSubscriptions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ExamSubscription examSubscription = db.ExamSubscription.Find(id);
            db.ExamSubscription.Remove(examSubscription);
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
