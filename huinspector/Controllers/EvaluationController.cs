using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using huinspector.Models;
using huinspector.Catalog;
using System.IO;
using System.Data.Entity.Infrastructure;

namespace huinspector.Controllers
{
    public class EvaluationController : Controller
    {
        private HUInspectorEntities1 db = new HUInspectorEntities1();

        // GET: /Evaluatie/5
        [Authorize]
        public ActionResult Index()
        {

            var eval = EvalCatalog.GetAll();
            return View(eval);
        }


        // GET: /Evaluatie/Details/5
        [Authorize]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var eval = EvalCatalog.GetEval(id);

            int check = check_Eval(id);

            if (check == 1)
            {
                ViewBag.Message = "Er zijn niet voldoende evaluaties!";
            }
            else if (check == 2)
            {
                ViewBag.Message = "De evaluatie is niet door voldoende personen uitgevoerd!";
            }
            else if (check == 3)
            {
                ViewBag.Message = "De evaluatie is nog niet door de administratie ingevuld!";
            }
            else if (check == 4)
            {
                ViewBag.Message = "De evaluatie is nog niet door een docent ingevuld!";
            }
            else
            {
                //Controles zijn goed, gemiddelde uitrekenen
                decimal result = Eval_Result(id);
                if (result != 0)
                {
                    ViewBag.Result = result;
                }
            }

            ViewBag.ExamId = id;
            return View(eval);
        }
        public int check_Eval(int? id)
        {
            int result = 0;

            //Check voldoende evaluaties
            var eval = EvalCatalog.GetEval(id);
            if (eval.Count < 5)
                result = 1;

            //check voldoende leerlingen
            int Students = EvalCatalog.CheckUsers(id, 3);
            if (Students < 3)
                result = 2;

            //Check voldoende administratie
            int Administration = EvalCatalog.CheckUsers(id, 2);
            if (Administration < 1)
                result = 3;

            //Check voldoende docent
            int Docents = EvalCatalog.CheckUsers(id, 1);
            if (Docents < 1)
                result = 4;

            return result;
        }

        public decimal Eval_Result(int? id)
        {
            decimal result = 0;

            var eval = EvalCatalog.GetEval(id);

            foreach (var value in eval)
            {
                result = result + (decimal)value.Result;
            }
            return result / eval.Count;
        }

        // GET: /Evaluatie/Create
        public ActionResult Create(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ViewBag.UserId = new SelectList(db.User, "Id", "FirstName");
            ViewBag.ExamName = ExamCatalog.GetName(id);
            //Exam vast voorvullen, deze wordt niet getoond maar is wel verplicht
            var model = new Evaluation
            {
                ExamId = (int)id
            };
            return View(model);
        }

        // POST: /Evaluatie/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UserId,ExamId,Result,Id")] Evaluation evaluation, HttpPostedFileBase upload)
        {

            if (ModelState.IsValid)
            {
                if (upload != null && upload.ContentLength > 0)
                {


                    byte[] filelength = new byte[upload.InputStream.Length];
                    byte[] file = new byte[upload.InputStream.Read(filelength, 0, filelength.Length)];
                    evaluation.Document = file;
                }
                db.Evaluation.Add(evaluation);
                db.SaveChanges(); //Hier gaat die fout  
                return RedirectToAction("Details", new { id = evaluation.ExamId });
            }

            ViewBag.ExamId = new SelectList(db.Exam, "Id", "Name", evaluation.ExamId);
            ViewBag.UserId = new SelectList(db.User, "Id", "FirstName", evaluation.UserId);
            return View(evaluation);
        }

        public FileResult Download(int? id)
        {
            byte[] file = EvalCatalog.GetFile(id);
            return File(file, "text/docx", "Evaluatie.docx");

        }

        // GET: /Evaluatie/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Evaluation evaluation = db.Evaluation.Find(id);
            if (evaluation == null)
            {
                return HttpNotFound();
            }
            ViewBag.ExamId = new SelectList(db.Exam, "Id", "Name", evaluation.ExamId);
            ViewBag.UserId = new SelectList(db.User, "Id", "FirstName", evaluation.UserId);
            return View(evaluation);
        }

        // POST: /Evaluatie/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UserId,ExamId,Result,Id")] Evaluation evaluation)
        {
            if (ModelState.IsValid)
            {
                db.Entry(evaluation).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ExamId = new SelectList(db.Exam, "Id", "Name", evaluation.ExamId);
            ViewBag.UserId = new SelectList(db.User, "Id", "FirstName", evaluation.UserId);
            return View(evaluation);
        }

        // GET: /Evaluatie/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Evaluation evaluation = db.Evaluation.Find(id);
            if (evaluation == null)
            {
                return HttpNotFound();
            }
            return View(evaluation);
        }

        // POST: /Evaluatie/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Evaluation evaluation = db.Evaluation.Find(id);
            db.Evaluation.Remove(evaluation);
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
