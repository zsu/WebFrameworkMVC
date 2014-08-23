using BrockAllen.MembershipReboot.Nh;
using Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Web.Areas.Admin.Models;
using App.Common.SessionMessage;

namespace Web.Areas.Admin.Controllers
{
    [Authorize(Roles = Constants.ROLE_ADMIN + "," + Constants.ROLE_USERADMIN + "," + Constants.PERMISSION_EDIT_USER)]
    public class UserController : Controller
    {
        private IUserService _userService;
        private IRoleService _roleService;
        public UserController(IUserService userService, IRoleService roleService)
        {
            _userService = userService;
            _roleService = roleService;
        }
        //
        // GET: /Admin/UserRole/
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
            bool isAdmin = User.IsInRole(Constants.ROLE_ADMIN);
            NhUserAccount user = _userService.GetById(new Guid(id));
            if (user == null)
            {
                SessionMessageManager.SetMessage(MessageType.Error, MessageBehaviors.StatusBar, string.Format("Invalid user id: {0}", id));
                return RedirectToAction("Index");
            }
            List<Role> roles = _roleService.GetRolesForUser(user.ID);
            if (roles != null && roles.Any(x => x.Name == Constants.ROLE_ADMIN) && !isAdmin)
            {
                SessionMessageManager.SetMessage(MessageType.Error, MessageBehaviors.StatusBar, "Permission denied");
                return RedirectToAction("Index");
            }
            UserRoleEditModel userRoleEditModel = new UserRoleEditModel { User = user };
            //foreach (Role role in allRoles)
            //{
            //    bool hasRole = user.Roles.AsQueryable().Any(x => x.Id == role.Id);
            //    userRoleEditModel.Roles.Add(new UserRoleModel { Role = role, HasRole = hasRole });
            //}
            return View(userRoleEditModel);
        }
    }
}