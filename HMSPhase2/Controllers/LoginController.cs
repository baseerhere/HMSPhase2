using HMSPhase2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace HMSPhase2.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }
        

        // POST: Login/Create
        [HttpPost]
        public ActionResult Index(Login model, string returnUrl)
        {
            try
            {
                if (model.EmailAddress.ToUpper() == "BASEERHERE@GMAIL.COM" && model.Password == "password")
                {
                    // TODO: Add insert logic here
                    FormsAuthentication.SetAuthCookie(model.EmailAddress, false);
                    if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                        && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                return View();
                
            }
            catch
            {
                return View();
            }
        }

        
    }
}
