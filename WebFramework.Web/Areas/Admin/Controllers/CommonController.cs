using App.Common;
using App.Common.Caching;
using App.Common.InversionOfControl;
using App.Common.SessionMessage;
using System.Web.Mvc;


namespace Web.Areas.Admin.Controllers
{
    [Authorize(Roles = Constants.ROLE_ADMIN)]
    public class CommonController : Controller
    {
        public CommonController()
        {
        }

        public ActionResult RestartApplication()
        {
            //restart application
            Util.RestartAppDomain();
            SessionMessageManager.SetMessage(MessageType.Success, MessageBehaviors.StatusBar, "Application is restarted successfully.");
            return RedirectToAction("Index", "Log");
        }
        public ActionResult ClearCache()
        {
            var cacheManager = IoC.GetService<ICacheManager>(Util.ApplicationCacheKey);
            cacheManager.Clear();
            SessionMessageManager.SetMessage(MessageType.Success, MessageBehaviors.StatusBar, "Cache is cleared successfully.");
            return RedirectToAction("Index", "Log");
        }
    }
}
