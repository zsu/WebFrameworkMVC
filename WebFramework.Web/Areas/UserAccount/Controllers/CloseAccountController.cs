using BrockAllen.MembershipReboot;
using BrockAllen.MembershipReboot.Hierarchical;
using BrockAllen.MembershipReboot.Nh;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Web.Areas.UserAccount.Controllers
{
    [Authorize]
    public class CloseAccountController : Controller
    {
        UserAccountService<NhUserAccount> userAccountService;
        public CloseAccountController(UserAccountService<NhUserAccount> userAccountService)
        {
            this.userAccountService = userAccountService;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(string button)
        {
            if (button == "yes")
            {
                try
                {
                    this.userAccountService.DeleteAccount(User.GetUserID());
                    return RedirectToAction("Index", "Logout");
                }
                catch (ValidationException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            return View();
        }

    }
}
