﻿using BrockAllen.MembershipReboot.Nh;
using Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Web.Areas.Admin.Controllers
{
    [Authorize(Roles = Constants.ROLE_ADMIN)]
    public class PermissionController : Controller
    {
        public PermissionController()
        {
        }
        //
        // GET: /Admin/Permission/
        public ActionResult Index()
        {
            return View();
        }
    }
}
