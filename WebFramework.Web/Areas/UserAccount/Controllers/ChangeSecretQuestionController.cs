using Web.Areas.UserAccount.Models;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Linq;
using System;
using BrockAllen.MembershipReboot.Hierarchical;
using BrockAllen.MembershipReboot;
using BrockAllen.MembershipReboot.Nh;

namespace Web.Areas.UserAccount.Controllers
{
    [Authorize]
    public class ChangeSecretQuestionController : Controller
    {
        UserAccountService<NhUserAccount> _userAccountService;
        public ChangeSecretQuestionController(UserAccountService<NhUserAccount> userAccountService)
        {
            _userAccountService = userAccountService;
        }
        
        public ActionResult Index()
        {
            var account = _userAccountService.GetByID(User.GetUserID());
            var vm = new PasswordResetSecretsViewModel
            {
                Secrets = account.PasswordResetSecrets.ToArray()
            };
            return View("Index", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Remove(Guid id)
        {
            _userAccountService.RemovePasswordResetSecret(User.GetUserID(), id);
            return RedirectToAction("Index");
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(AddSecretQuestionInputModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var account = _userAccountService.GetByID(User.GetUserID());
                    if (_userAccountService.Authenticate(account.Username, model.Password))
                    {
                        _userAccountService.AddPasswordResetSecret(account.ID, model.Question, model.Answer);
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError("Password", "Wrong password");
                    }
                }
                catch (ValidationException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return View("Add", model);
        }
    }
}
