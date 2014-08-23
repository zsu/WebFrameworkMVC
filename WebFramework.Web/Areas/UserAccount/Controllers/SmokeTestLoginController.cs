using BrockAllen.MembershipReboot.Hierarchical;
using Web.Areas.UserAccount.Models;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens;
using System.Security.Cryptography.X509Certificates;
using System.Web.Mvc;
using BrockAllen.MembershipReboot;
using BrockAllen.MembershipReboot.Nh;
using Web.FilterAttributes;

namespace Web.Areas.UserAccount.Controllers
{
    [AllowAnonymous]
    [MaintenanceMessagesFilter(Disabled = true)]
    [MvcMaintenanceMessagesFilter(Disabled = true)]
    public class SmokeTestLoginController : Controller
    {
        UserAccountService<NhUserAccount> userAccountService;
        AuthenticationService<NhUserAccount> authSvc;
        public SmokeTestLoginController(AuthenticationService<NhUserAccount> authSvc)
        {
            this.userAccountService = authSvc.UserAccountService;
            this.authSvc = authSvc;
        }
        public ActionResult Index(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View(new LoginInputModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(LoginInputModel model)
        {
            if (ModelState.IsValid)
            {
                NhUserAccount account;
                if (userAccountService.Authenticate(model.Username, model.Password, out account))//userAccountService.AuthenticateWithUsernameOrEmail(model.Username, model.Password, out account))
                {
                    authSvc.SignIn(account, model.RememberMe);

                    if (account.RequiresTwoFactorAuthCodeToSignIn())
                    {
                        return RedirectToAction("TwoFactorAuthCodeLogin");
                    }
                    if (account.RequiresTwoFactorCertificateToSignIn())
                    {
                        return RedirectToAction("CertificateLogin");
                    }

                    if (account.RequiresPasswordReset || userAccountService.IsPasswordExpired(account))
                    {
                        return RedirectToAction("Index", "ChangePassword");
                    }

                    if (Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }

                    return RedirectToAction("Index", "Home", new { area = "" });
                }
                else
                {
                    ModelState.AddModelError("", "Invalid Username or Password");
                }
            }

            return View(model);
        }
    }
}
