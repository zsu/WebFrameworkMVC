﻿using Web.Areas.UserAccount.Models;
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
        UserAccountService<NhUserAccount> userAccountService;
        public ChangeSecretQuestionController(UserAccountService<NhUserAccount> userAccountService)
        {
            this.userAccountService = userAccountService;
        }
        
        public ActionResult Index()
        {
            var account = this.userAccountService.GetByID(User.GetUserID());
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
            this.userAccountService.RemovePasswordResetSecret(User.GetUserID(), id);
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
                    this.userAccountService.AddPasswordResetSecret(User.GetUserID(), model.Password, model.Question, model.Answer);
                    return RedirectToAction("Index");
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
