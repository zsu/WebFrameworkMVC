using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            if (User.IsInRole(Constants.ROLE_ADMIN))
                return RedirectToAction("Index", "Log", new { area = "Admin" });
            else if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home", new { area = "UserAccount" });
            return View();
        }

        public ActionResult About()
        {

            return View();
        }

        public ActionResult Contact()
        {

            return View();
        }
    }
}