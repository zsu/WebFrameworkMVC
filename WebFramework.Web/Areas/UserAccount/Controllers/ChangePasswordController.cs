using Web.Areas.UserAccount.Models;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using BrockAllen.MembershipReboot;
using BrockAllen.MembershipReboot.Nh;
using System;

namespace Web.Areas.UserAccount.Controllers
{
    //[Authorize]
    public class ChangePasswordController : Controller
    {
        UserAccountService<NhUserAccount> userAccountService;
        public ChangePasswordController(UserAccountService<NhUserAccount> userAccountService)
        {
            this.userAccountService = userAccountService;
        }
        
        public ActionResult Index(Guid? uid)
        {
            if (!User.HasUserID() && uid==null)
            {
                return new HttpUnauthorizedResult();
            }
            return View(new ChangePasswordInputModel());
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(ChangePasswordInputModel model,Guid? uid)
        {
            if (!User.HasUserID() && uid==null)
            {
                return new HttpUnauthorizedResult();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    this.userAccountService.ChangePassword(User.HasUserID()?User.GetUserID():uid.Value, model.OldPassword, model.NewPassword);
                    return View("Success");
                }
                catch (ValidationException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return View(model);
        }
    }
}
