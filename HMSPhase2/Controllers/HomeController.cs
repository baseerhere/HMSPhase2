using System.Web.Mvc;

namespace HMSPhase2.Controllers
{
    public class HomeController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public ActionResult AnotherLink()
        {
            return View("Index");
        }

        [Authorize]
        public ActionResult Restaurants()
        {
            return View("Restaurants");
        }

        [Authorize]
        public ActionResult CreateRestaurant()
        {
            return View("CreateRestaurant");
        }
    }
}
