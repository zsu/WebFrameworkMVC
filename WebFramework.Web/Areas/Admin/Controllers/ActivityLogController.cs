
using App.Common.Logging;
using Elmah;
using Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Areas.Admin.Models;
using WebFramework.Data.Domain;

namespace Web.Areas.Admin.Controllers
{
    [Authorize(Roles=Constants.ROLE_ADMIN)]//TODO: Change to ManageUser role
    public class ActivityLogController : Controller
    {
        public ActivityLogController()
        {
        }

        public ActionResult Index()
        {
            return View();
        }
	}
}