using System.Web.Mvc;

namespace Web.Areas.Admin.Controllers
{
    [Authorize(Roles = Constants.ROLE_ADMIN)]
    public class MessageTemplateController : Controller
    {
        public MessageTemplateController()
        {
        }
        //
        // GET: /Admin/Permission/
        public ActionResult Index()
        {
            return View();
        }
    }
}
