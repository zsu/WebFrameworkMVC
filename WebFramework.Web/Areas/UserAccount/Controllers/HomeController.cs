using BrockAllen.MembershipReboot;
using BrockAllen.MembershipReboot.Nh;
using System;
using System.Security.Claims;
using System.Web.Mvc;

namespace Web.Areas.UserAccount.Controllers
{
    public class HomeController : Controller
    {
        UserAccountService<NhUserAccount> userAccountService;
        AuthenticationService<NhUserAccount> authSvc;

        public HomeController(AuthenticationService<NhUserAccount> authSvc)
        {
            this.userAccountService = authSvc.UserAccountService;
            this.authSvc = authSvc;
        }

        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult Index(string gender)
        {
            if (String.IsNullOrWhiteSpace(gender))
            {
                userAccountService.RemoveClaim(User.GetUserID(), ClaimTypes.Gender);
            }
            else
            {
                // if you only want one of these claim types, uncomment the next line
                //account.RemoveClaim(ClaimTypes.Gender);
                userAccountService.AddClaim(User.GetUserID(), ClaimTypes.Gender, gender);
            }

            // since we've changed the claims, we need to re-issue the cookie that
            // contains the claims.
            authSvc.SignIn(User.GetUserID());

            return RedirectToAction("Index");
        }

    }
}
