﻿using Web.Areas.UserAccount.Models;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using BrockAllen.MembershipReboot;
using BrockAllen.MembershipReboot.Nh;

namespace Web.Areas.UserAccount.Controllers
{
    [Authorize]
    public class ChangeEmailController : Controller
    {
        UserAccountService<NhUserAccount> userAccountService;
        AuthenticationService<NhUserAccount> authSvc;

        public ChangeEmailController(AuthenticationService<NhUserAccount> authSvc)
        {
            this.userAccountService = authSvc.UserAccountService;
            this.authSvc = authSvc;
        }

        public ActionResult Index()
        {
            return View("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(ChangeEmailRequestInputModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    this.userAccountService.ChangeEmailRequest(User.GetUserID(), model.NewEmail);
                    if (userAccountService.Configuration.RequireAccountVerification)
                    {
                        return View("ChangeRequestSuccess", (object)model.NewEmail);
                    }
                    else
                    {
                        return View("Success");
                    }
                }
                catch (ValidationException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            return View("Index", model);
        }

        [AllowAnonymous]
        public ActionResult Confirm(string id)
        {
            var account = this.userAccountService.GetByVerificationKey(id);
            if (account == null)
                return View("Error");
            if (account.HasPassword())
            {
                var vm = new ChangeEmailFromKeyInputModel();
                vm.Key = id;
                return View("Confirm", vm);
            }
            else
            {
                try
                {
                    userAccountService.VerifyEmailFromKey(id, out account);
                    // since we've changed the email, we need to re-issue the cookie that
                    // contains the claims.
                    authSvc.SignIn(account);
                    return RedirectToAction("Success");
                }
                catch (ValidationException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
                return View("Confirm", null);
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Confirm(ChangeEmailFromKeyInputModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    NhUserAccount account;
                    this.userAccountService.VerifyEmailFromKey(model.Key, model.Password, out account);
                    
                    // since we've changed the email, we need to re-issue the cookie that
                    // contains the claims.
                    authSvc.SignIn(account);
                    return RedirectToAction("Success");
                }
                catch (ValidationException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            
            return View("Confirm", model);
        }

        public ActionResult Success()
        {
            return View();
        }
    }
}
