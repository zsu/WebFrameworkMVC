using Service;
using System.Web.Mvc;

namespace Web.Areas.Admin.Controllers
{
    [Authorize(Roles=Constants.ROLE_ADMIN)]//TODO: Change to ManageUser role
    public class LogController : Controller
    {
        private ILogService _logService;
        public LogController(ILogService logService)
        {
            _logService = logService;
        }
        //
        // GET: /Admin/UserRole/
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Detail(long id)
        {
            if (id==default(long))
                return RedirectToAction("Index");
            var log = _logService.GetLogById(id);
            return View(log);
        }
	}
}