
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
    public class ElmahLogController : Controller
    {
        private IElmahLogService _logService;
        public ElmahLogController(IElmahLogService logService)
        {
            _logService = logService;
        }
        //
        // GET: /Admin/UserRole/
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Detail(string id)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectToAction("Index");
            var log = _logService.GetElmahLogById(id);
            return View(log);
        }
	}
}