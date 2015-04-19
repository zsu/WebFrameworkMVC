using BrockAllen.MembershipReboot.Hierarchical;
using Web.Areas.UserAccount.Models;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using BrockAllen.MembershipReboot;
using BrockAllen.MembershipReboot.Nh;
using Service;

namespace Web.Areas.UserAccount.Controllers
{
    [AllowAnonymous]
    public class RegisterController : Controller
    {
        UserAccountService<NhUserAccount> _userAccountService;
        AuthenticationService<NhUserAccount> _authSvc;
        IUserService _userService;

        public RegisterController(AuthenticationService<NhUserAccount> authSvc,IUserService userService)
        {
            this._authSvc = authSvc;
            this._userAccountService = authSvc.UserAccountService;
            _userService = userService;
        }

        public ActionResult Index()
        {
            return View(new RegisterInputModel());
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Index(RegisterInputModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    NhUserAccount account = new NhUserAccount { Username = model.Username, HashedPassword = model.Password, Email = model.Email, FirstName = model.FirstName, LastName = model.LastName };
                    account = _userService.CreateAccount(account);
                    //account.FirstName = model.FirstName;
                    //if (!string.IsNullOrWhiteSpace(model.LastName))
                    //    account.LastName = model.LastName;
                    //this.userAccountService.Update(account);
                    ViewData["RequireAccountVerification"] = this._userAccountService.Configuration.RequireAccountVerification;
                    return View("Success", model);
                }
                catch (ValidationException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return View(model);
        }

        public ActionResult Verify()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Verify(string foo)
        {
            try
            {
                this._userAccountService.RequestAccountVerification(User.GetUserID());
                return View("Success");
            }
            catch (ValidationException ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            return View();
        }

        public ActionResult Cancel(string id)
        {
            try
            {
                bool closed;
                _userService.CancelVerification(id, out closed);
                if (closed)
                {
                    return View("Closed");
                }
                else
                {
                    return View("Cancel");
                }
            }
            catch(ValidationException ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            return View("Error");
        }
    }
}
