using Service;
using System.Web.Mvc;
using Web.FilterAttributes;

namespace Web.Controllers
{
    [MaintenanceMessagesFilter(Disabled=true)]
    [MvcMaintenanceMessagesFilter(Disabled=true)]
    public class MaintenanceController : Controller
    {
        private ISettingService _settingService;
        public MaintenanceController(ISettingService settingService)
        {
            _settingService = settingService;
        }
        public ActionResult Index()
        {
            var maintenanceMessage = _settingService.GetSettingByKey<string>(Constants.SETTING_KEYS_MAINTENANCE_MESSAGE, "The site is under maintenance.");
            ViewBag.Message = maintenanceMessage;
            return View();
        }
    }
}