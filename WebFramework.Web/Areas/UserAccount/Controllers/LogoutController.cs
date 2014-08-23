using WebFramework.Data.Domain;
using App.Common;
using BrockAllen.MembershipReboot;
using BrockAllen.MembershipReboot.Hierarchical;
using BrockAllen.MembershipReboot.Nh;
using Service;
using System;
using System.Web.Mvc;

namespace Web.Areas.UserAccount.Controllers
{
    public class LogoutController : Controller
    {
        AuthenticationService<NhUserAccount> _authSvc;
        IAuthenticationAuditService _authenticationAuditService;
        public LogoutController(AuthenticationService<NhUserAccount> authSvc,IAuthenticationAuditService authenticationAuditService)
        {
            _authSvc = authSvc;
            _authenticationAuditService = authenticationAuditService;
        }
        
        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                var audit = new AuthenticationAudit
                {
                    Application = Util.ApplicationConfiguration.AppAcronym,
                    UserName = User.Identity.Name,
                    CreatedDate = DateTime.UtcNow,
                    Activity = "LogoutSuccess",
                    Detail = string.Format("User {0} logout successfully.",User.Identity.Name),
                    ClientIP = Request.UserHostAddress,
                };
                _authSvc.SignOut();
                _authenticationAuditService.Add(audit); 
                return RedirectToAction("Index");
            }
            
            return View();
        }

    }
}
