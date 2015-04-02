using huinspector.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace huinspector.Controllers
{
    public class LoginController : Controller
    {
        //
        // GET: /Login/
        public ActionResult Index()
        {
            return View();
        }


        [Authorize]
        public ActionResult Dashboard()
        {
            return View();
        }

        [HttpGet]
        public ActionResult LogIn(string returnURL)
        {
            ViewBag.ReturnURL = returnURL;
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(User model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                using (HUInspectorEntities entities = new HUInspectorEntities())
                {
                    int accountStatus = CheckAccount(model.FirstName, model.Password,ref model);

                    if (accountStatus == 1)
                    {
                        FormsAuthentication.SetAuthCookie(model.FirstName, false);
                        Session.Add("CurrentLoggedInUserID", model.Id);
                        Session.Add("CurrentLoggedInUserName", model.FirstName);
                        if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/") && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                        {
                            return Redirect(returnUrl);
                        }
                        else
                        {
                            //Default to the dashboard
                            return RedirectToAction("Index", "Home");
                        }
                    }
                    else if (accountStatus == 2)
                    {
                        ModelState.AddModelError("", "Bij de door u ingevoerde gegevens zijn geen accountgegevens gevonden. Controleer uw invoer en probeer nogmaals.");
                    }
                    else if (accountStatus == 3)
                    {
                        ModelState.AddModelError("", "Dit account heeft geen toegang tot het studentengedeelte");
                    }
                }
            }
            return View();
        }
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        private int CheckAccount(string firstname, string pass, ref User model)
        {
            string username = firstname;
            string password = pass;
            int accountValid = 2;

            using (HUInspectorEntities entities = new HUInspectorEntities())
            {
                model = null;

                if (entities.User.Any(user => user.FirstName == username && user.Password == password))
                    model = entities.User.First(user => user.FirstName == username && user.Password == password);

                bool userValid = model != null;

                if (userValid)
                {
                    //Account bestaat, maar mag hij ook inloggen?
                    bool accountAcces = entities.User.Any(user => user.FirstName == username && user.Password == password && user.UserTypeId == 3);
                    if (accountAcces)
                    {
                        accountValid = 1; //Account heeft toegang
                    }
                    else
                    {
                        accountValid = 3; //Geen rechten
                    }
                }
                else
                {
                    accountValid = 2; //Accountgegevens niet gevonden
                }
            }
            return accountValid;
        }
    }
}