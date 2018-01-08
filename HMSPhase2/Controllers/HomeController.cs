using System.Web.Mvc;

namespace HMSPhase2.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AnotherLink()
        {
            return View("Index");
        }

        public ActionResult Restaurants()
        {
            return View("Restaurants");
        }

        public ActionResult CreateRestaurant()
        {
            return View("CreateRestaurant");
        }
    }
}
