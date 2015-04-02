using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using huinspector.Models;
using huinspector.Catalogs;
using System.IO;
using System.Data.Entity.Infrastructure;
using System.Net.Mail;

namespace huinspector.Controllers
{
    public class EvaluationController : Controller
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

        public ActionResult SubmitExamEvaluation(int? id)
        {
            ViewData["ExamID"] = id;
            return View();
        }

        [HttpPost]
        public ActionResult Insert(EvaluationInsertModel Insertmodel)
        {
            Evaluation eval = new Evaluation();
            eval.ExamId = Insertmodel.ExamId;
            User loggedUser = UserCatalog.GetUser(CurrentUserID);
            eval.UserId = CurrentUserID;

            using (var binaryReader = new BinaryReader(Insertmodel.Document.InputStream))
            {
                eval.Document = binaryReader.ReadBytes(Insertmodel.Document.ContentLength);
            }

            eval.Result = Insertmodel.Result;
            if (Insertmodel.Document != null)
            {
                eval.Mimetype = Insertmodel.Document.ContentType;
            }
            EvaluationCatalog.SetEvaluation(eval);

             var exam = ExamCatalog.GetExam(Insertmodel.ExamId);


            MailMessage mail = new MailMessage();
            mail.To.Add("uithuizermeeden.kwaliteit@gmail.com");
            mail.From = new MailAddress("huinspector@tfknulst.nl", "HU Inspector");
            mail.Subject = "Nieuwe evaluatie";
            string body = "Geachte kwaliteitsco&ouml;rdinator, <br><br> Er is zojuist een nieuwe evaluatie ingevoerd voor het tentamen <b>" + exam.Name + "</b>. ";
            body += "De evaluatie is ingevoerd door <b>" + loggedUser.FirstName + " " + loggedUser.Insertion + " " + loggedUser.LastName + "</b>. ";
            body += " Hij of zij gaf het tentamen een <b> " + Insertmodel.Result + "</b>.";

            if (Insertmodel.Document != null)
            {
                body += "Het document wat geupload is bij de evaluatie vind u in de bijlage. ";
            }
            else
            {
                body += "Er is geen document geupload bij de evaluatie. Deze kunnen we u zodoende niet doen toekomen. ";
            }

            body += "<br><br> Met vriendelijke groet, <br>HU Inspector<br><br><i>Dit is een automatisch gegenereerde e-mail, u kunt hier niet op reageren.</i>";
            mail.IsBodyHtml = true;
            mail.Body = body;

            if (Insertmodel.Document != null)
            {
                Attachment attachment = new Attachment(Insertmodel.Document.InputStream, Insertmodel.Document.FileName);
                mail.Attachments.Add(attachment);
            }

            SmtpClient smtp = new SmtpClient();
            smtp.Host = "mail.tfknulst.nl";
            smtp.Port = 587;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new System.Net.NetworkCredential("huinspector@tfknulst.nl", "tpJQ6Ell");
            smtp.EnableSsl = false;
            smtp.Send(mail);


            return RedirectToAction("/Index", "Exams");
        }
    }
}
