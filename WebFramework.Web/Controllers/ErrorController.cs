using System.Web.Mvc;

namespace Web.Controllers
.Controllers
{
    public class ErrorController : Controller
    {
        public ActionResult Index()
        {
            return View("Error");
        }
    }
}