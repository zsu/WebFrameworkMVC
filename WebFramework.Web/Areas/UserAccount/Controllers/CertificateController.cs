﻿using BrockAllen.MembershipReboot;
using BrockAllen.MembershipReboot.Nh;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Web.Mvc;

namespace Web.Areas.UserAccount.Controllers
{
    [Authorize]
    public class CertificateController : Controller
    {
        UserAccountService<NhUserAccount> userAccountService;

        public CertificateController(UserAccountService<NhUserAccount> userAccountService)
        {
            this.userAccountService = userAccountService;
        }

        public ActionResult Index(Guid? uid)
        {
            var acct = userAccountService.GetByID(User.HasUserID() ? User.GetUserID() : uid.Value);
            return View(acct);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string thumbprint)
        {
            userAccountService.RemoveCertificate(this.User.GetUserID(), thumbprint);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add()
        {
            var acct = userAccountService.GetByID(this.User.GetUserID());

            if (Request.Files.Count == 0)
            {
                ModelState.AddModelError("", "No file uploaded");
            }
            else
            {
                try
                {
                    using (var ms = new MemoryStream())
                    {
                        Request.Files[0].InputStream.CopyTo(ms);
                        var bytes = ms.ToArray();

                        var cert = new X509Certificate2(bytes);
                        userAccountService.AddCertificate(User.GetUserID(), cert);

                        return RedirectToAction("Index");
                    }
                }
                catch (ValidationException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Error processing certificate");
                }
            }

            return View("Index", acct);
        }
    }
}
