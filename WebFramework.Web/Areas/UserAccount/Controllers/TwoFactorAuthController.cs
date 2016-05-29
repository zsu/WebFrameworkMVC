using BrockAllen.MembershipReboot;
using BrockAllen.MembershipReboot.Nh;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Web.Areas.UserAccount.Controllers
{
    public class TwoFactorAuthController : Controller
    {
        UserAccountService<NhUserAccount> userAccountService;

        public TwoFactorAuthController(UserAccountService<NhUserAccount> userAccountService)
        {
            this.userAccountService = userAccountService;
        }

        public ActionResult Index(Guid? uid)
        {
            var acct = userAccountService.GetByID(User.HasUserID()?this.User.GetUserID():uid.Value);
            return View(acct);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(TwoFactorAuthMode mode,Guid? uid)
        {
            try
            {
                this.userAccountService.ConfigureTwoFactorAuthentication(User.HasUserID()?User.GetUserID():uid.Value, mode);
                
                ViewData["Message"] = "Update Success";
                
                var acct = userAccountService.GetByID(User.HasUserID() ? User.GetUserID() : uid.Value);
                return View("Index", acct);
            }
            catch (ValidationException ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            
            return View("Index", userAccountService.GetByID(User.HasUserID() ? User.GetUserID() : uid.Value));
        }
    }
}
