﻿using System;
using System.Web.Mvc;

namespace Web.Areas.UserAccount.Controllers
{
    public class ExceptionController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            throw new Exception("Exception test.");
        }

    }
}
