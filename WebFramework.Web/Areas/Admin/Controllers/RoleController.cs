using BrockAllen.MembershipReboot.Nh;
using Service;
using System;
using System.Web.Mvc;

namespace Web.Areas.Admin.Controllers
{
    [Authorize(Roles = Constants.ROLE_ADMIN + "," + Constants.PERMISSION_EDIT_ROLE)]
    public class RoleController : Controller
    {
        private IRoleService _roleService;
        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }
        //
        // GET: /Admin/Role/
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("Index");
            }
            Role role = _roleService.GetById(new Guid(id));
            return View(role);
        }
        public ActionResult UserList()
        {
            return View();
        }
    }
}
