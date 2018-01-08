using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HMSPhase2.Controllers
{
    public class RestaurantController : Controller
    {
        // GET: Restaurant
        public ActionResult Index()
        {
            return View("List");
        }

        public ActionResult List()
        {
            return View("List");
        }

        public ActionResult Create()
        {
            return View("Create");
        }
    }
}