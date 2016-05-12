using Service;
using System.Web.Mvc;

namespace Web.Areas.Admin.Controllers
{
    [Authorize(Roles=Constants.ROLE_ADMIN)]//TODO: Change to ManageUser role
    public class ElmahLogController : Controller
    {
        private IElmahLogService _logService;
        public ElmahLogController(IElmahLogService logService)
        {
            _logService = logService;
        }
        //
        // GET: /Admin/UserRole/
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Detail(string id)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectToAction("Index");
            var log = _logService.GetElmahLogById(id);
            return View(log);
        }
	}
}