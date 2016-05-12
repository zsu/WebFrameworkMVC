using System.Web.Mvc;

namespace Web.Areas.Admin.Controllers
{
    [Authorize(Roles=Constants.ROLE_ADMIN)]//TODO: Change to ManageUser role
    public class ActivityLogController : Controller
    {
        public ActivityLogController()
        {
        }

        public ActionResult Index()
        {
            return View();
        }
	}
}