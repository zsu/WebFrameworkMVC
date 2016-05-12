using System.Web.Mvc;

namespace Web.Areas.Admin.Controllers
{
    [Authorize(Roles=Constants.ROLE_ADMIN)]//TODO: Change to ManageUser role
    public class AuthenticationAuditController : Controller
    {
        public AuthenticationAuditController()
        {
        }

        public ActionResult Index()
        {
            return View();
        }
	}
}