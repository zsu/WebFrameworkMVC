using BrockAllen.MembershipReboot.Nh;
using Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Linq.Dynamic;
using App.Common.InversionOfControl;
using App.Mvc.JqGrid;
using System.Text;
using System.Web.Http.ModelBinding;
using App.Mvc.ModelBinder;
using Web.Areas.Admin.Models;
using System.IO;

namespace Web.Controllers.Api
{
    [Authorize(Roles = Constants.ROLE_ADMIN + "," + Constants.PERMISSION_EDIT_ROLE)]
    public class RoleController : ApiController
    {
        private IRoleService _service;
        private IPermissionService _permissionService;
        private IUserService _userService;
        public RoleController()
        {
            _service = IoC.GetService<IRoleService>();
            _permissionService = IoC.GetService<IPermissionService>();
            _userService = IoC.GetService<IUserService>();
        }
        // GET api/role
        public dynamic GetGridData([FromUri] JqGridSearchModel searchModel)
        {
            var data = Web.Infrastructure.Util.GetGridData<Role>(searchModel, _service.Query());
            var dataList = data.Items.Select(x => new { x.Id, x.Name, x.Description }).ToList(); 
            var grid = new JqGridModel
            {
                total = data.TotalPage,
                page = data.CurrentPage,
                records = data.TotalNumber,
                rows = dataList.Select(x => new { id = x.Id, cell = new object[] { x.Id, x.Name, x.Description } }).ToArray()
            };
            return grid;
        }
        [Route("api/role/exporttoexcel")]
        [HttpGet]
        public dynamic ExportToExcel([FromUri]JqGridSearchModel searchModel)
        {
            var query = _service.Query();
            //query = query.Where(x => x.Name.StartsWith(string.Format("{0}.", App.Common.Util.ApplicationConfiguration.AppAcronym)));
            searchModel.rows = 0;
            var data = Web.Infrastructure.Util.GetGridData<Role>(searchModel, query);
            var dataList = data.Items.Select(x => new { x.Name, x.Description }).ToList();
            string filePath = Web.Infrastructure.ExporterManager.Export("role", Web.Infrastructure.ExporterType.CSV, dataList.ToList(), "");
            HttpResponseMessage result = null;

            if (!File.Exists(filePath))
            {
                result = Request.CreateResponse(HttpStatusCode.Gone);
            }
            else
            {
                result = Request.CreateResponse(HttpStatusCode.OK);
                result.Content = new StreamContent(new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
                result.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/csv");
                result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
                result.Content.Headers.ContentDisposition.FileName = Path.GetFileName(filePath);
                result.Content.Headers.ContentLength = new FileInfo(filePath).Length;

            }
            return result;
        }
        // GET api/role/5
        public IHttpActionResult Get(Guid id)
        {
            var item = _service.Query().FirstOrDefault((p) => p.Id == id);
            if (item == null)
            {
                return NotFound();
            }
            return Ok(item);
        }

        // POST api/role
        public IHttpActionResult Post([FromBody] Role item)
        {
            StringBuilder message = new StringBuilder();
            if (item == null)
                return BadRequest("Role cannot be empty.");
            if (string.IsNullOrEmpty(item.Name))
                return BadRequest("Rolename cannot be empty.");
            item.Name = item.Name.Trim();
            item.Description = string.IsNullOrEmpty(item.Description) ? null : item.Description.Trim();
            if (_service.RoleExists(item.Name))
                return BadRequest(string.Format("Role {0} already exists.", item.Name));
            _service.CreateRole(item);
            //return Created<Role>(Request.RequestUri + item.Id.ToString(), item);
            message.AppendFormat("Role {0}  is saved successflly.", item.Name);
            return Json<object>(new { Success = true, Message = message.ToString(), RowId = item.Id });
        }

        // PUT api/role/5
        public IHttpActionResult Put(Guid id, [FromBody] Role item)
        {
            StringBuilder message = new StringBuilder();
            if (id == default(Guid))
                return BadRequest("Role id cannot be empty.");
            if (item==null)
            {
                return BadRequest("Role cannot be empty.");
            }
            if (string.IsNullOrEmpty(item.Name))
            {
                return BadRequest("Rolename cannot be empty.");
            }
            item.Id = id;
            item.Name = item.Name.Trim();
            item.Description=string.IsNullOrEmpty(item.Description)?null:item.Description.Trim();
            //if (_roleService.RoleExists(item.Name))
            //    return BadRequest(string.Format("Role {0} already exists.", item.Name));
            //Role role = new Role() { Id = item.Id, Name = item.Name, Description = item.Description };
            _service.UpdateRole(item);
            message.AppendFormat("Role {0}  is saved successflly.", item.Name);
            return Json<object>(new { Success = true, Message = message.ToString(), RowId=item.Id });

        }

        // DELETE api/role/5
        public IHttpActionResult Delete(Guid id)
        {
            if (id == default(Guid))
                return BadRequest("Role id cannot be empty.");
            var usersInRole = _service.GetUsersInRole(id);
            if (usersInRole != null && usersInRole.Count > 0)
                return BadRequest("Role is still being used and cannot be deleted.");
            if (_service.DeleteRole(id))
                return Ok();
            else
                return NotFound();
        }
        [Route("api/rolepermissions/{id}")]
        [HttpPut]
        public IHttpActionResult UpdateRolePermissions(Guid id, [ModelBinder(typeof(FieldValueModelBinder))] RolePermissionModel item)
        {
            StringBuilder message = new StringBuilder();
            if (id == default(Guid))
                return BadRequest("Role id cannot be empty.");
            if (item == null)
            {
                return BadRequest("Permission cannot be empty.");
            }

            bool hasPermission = item.HasPermission;
            Guid rId = id;
            string permissionName = item.Name;
            if (_permissionService.IsRoleAssignedPermission(rId, permissionName))
            {
                if (!hasPermission)
                {
                    _permissionService.RemovePermissionsFromRole(rId, new List<string>() { permissionName });
                    message.AppendFormat("Permission {0} has been removed from role successfully.", permissionName);
                }
            }
            else if (hasPermission)
            {
                _permissionService.AssignPermissionsToRole(rId, new List<string>() { permissionName });
                message.AppendFormat("Permission {0} is assigned to role successfully.", permissionName);
            }
            return Json<Object>(new { Success = true, Message = message.ToString(), RowId = item.Id });

        }
        [Route("api/rolepermissions/{id}")]
        public dynamic GetRolePermissions(Guid id, [FromUri] JqGridSearchModel searchModel)
        {
            if (id == default(Guid))
                return BadRequest("Role id cannot be empty.");
            Role role = _service.GetById(id);
            List<Permission> allPermission = _permissionService.GetAllPermissions();
            List<RolePermissionModel> rolePermissions = new List<RolePermissionModel>();
            foreach (var permission in allPermission)
            {
                bool hasPermission = role.Permissions.AsQueryable().Any(x => x.Id == permission.Id);
                rolePermissions.Add(new RolePermissionModel { Id = permission.Id, Name = permission.Name, Description = permission.Description, HasPermission = hasPermission });
            }
            var data = Web.Infrastructure.Util.GetGridData<RolePermissionModel>(searchModel, rolePermissions.AsQueryable());
            var dataList = data.Items.Select(x => new
            {
                x.Id,
                x.Name,
                x.Description,
                x.HasPermission
            }).ToList(); 
            var grid = new JqGridModel
            {
                total = data.TotalPage,
                page = data.CurrentPage,
                records = data.TotalNumber,
                rows = dataList.Select(x => new
                {
                    id = x.Id,
                    cell = new object[] {
                        x.Id,
                        x.Name,
                        x.Description,
                        x.HasPermission}
                }).ToArray()
            };
            return grid;
        }
        [Route("api/rolepermissions/exporttoexcel")]
        [HttpGet]
        public dynamic ExportToExcelRolePermissions(Guid id, [FromUri]JqGridSearchModel searchModel)
        {
            if (id == default(Guid))
                return BadRequest("Role id cannot be empty.");
            searchModel.rows = 0;
            int startRow = (searchModel.page * searchModel.rows) + 1;
            int skip = (searchModel.page > 0 ? searchModel.page - 1 : 0) * searchModel.rows;
            Role role = _service.GetById(id);
            List<Permission> allPermission = _permissionService.GetAllPermissions();
            List<RolePermissionModel> rolePermissions = new List<RolePermissionModel>();
            foreach (var permission in allPermission)
            {
                bool hasPermission = role.Permissions.AsQueryable().Any(x => x.Id == permission.Id);
                rolePermissions.Add(new RolePermissionModel { Id = permission.Id, Name = permission.Name, Description = permission.Description, HasPermission = hasPermission });
            }
            //note - these queries require "using System.Dynamic.Linq" library
            IQueryable<RolePermissionModel> query = rolePermissions.AsQueryable();
            var data = Web.Infrastructure.Util.GetGridData<RolePermissionModel>(searchModel, query);
            var dataList = data.Items.Select(x => new { x.Name, x.Description, x.HasPermission }).ToList();
            string filePath = Web.Infrastructure.ExporterManager.Export("rolepermissions", Web.Infrastructure.ExporterType.CSV, dataList, "");
            HttpResponseMessage result = null;

            if (!File.Exists(filePath))
            {
                result = Request.CreateResponse(HttpStatusCode.Gone);
            }
            else
            {
                result = Request.CreateResponse(HttpStatusCode.OK);
                result.Content = new StreamContent(new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
                result.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/csv");
                result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
                result.Content.Headers.ContentDisposition.FileName = Path.GetFileName(filePath);
                result.Content.Headers.ContentLength = new FileInfo(filePath).Length;

            }
            return result;
        }
        [Route("api/roleuserlist")]
        public dynamic GetRoleUserList([FromUri] JqGridSearchModel searchModel)
        {
            var query = _service.Query();
            var gridData = Web.Infrastructure.Util.GetGridData<Role>(searchModel, query);
            int totalRecords = gridData.TotalNumber;
            var data = gridData.Items;

            var dataListTemp = data.SelectMany(u => _userService.Query(), (r, u) => new { r, u })
                .Where(x => x.u.Roles.Contains(x.r)).OrderBy(x => x.r.Id).Select(x => new { x.r.Id, x.r.Name, x.r.Description, x.u }).ToList();
            var roles = data.ToList();
            List<RoleUserModel> dataList = new List<RoleUserModel>();
            foreach (Role role in roles)
            {
                RoleUserModel roleUser = new RoleUserModel { Id = role.Id, Description = role.Description, Name = role.Name };
                var users = dataListTemp.Where(x => x.Id == role.Id).Select(x => x.u).ToList();
                foreach (var user in users)
                    roleUser.Users.Add(new RoleUserModel.User { Id = user.ID, UserName = user.Username, FirstName = user.FirstName, LastName = user.LastName });
                dataList.Add(roleUser);
            }
            var totalPages = (int)Math.Ceiling((float)totalRecords / searchModel.rows);
            return new JqGridModel
            {
                total = searchModel.page > 0 ? totalPages : 1,
                page = searchModel.page,
                records = totalRecords,
                rows = dataList.Select(x => new { id = x.Id, cell = new object[] { x.Id, x.Name, x.Description, x.Users } }).ToArray()
            };
        }
        [Route("api/roleuserlist/exporttoexcel")]
        [HttpGet]
        public dynamic ExportToExcelRoleUserList([FromUri]JqGridSearchModel searchModel)
        {
            var query = _service.Query();
            searchModel.rows = 0;
            var gridData = Web.Infrastructure.Util.GetGridData<Role>(searchModel, query);
            int totalRecords = gridData.TotalNumber;
            var data = gridData.Items;

            var dataListTemp = data.SelectMany(u => _userService.Query(), (r, u) => new { r, u })
                .Where(x => x.u.Roles.Contains(x.r)).OrderBy(x => x.r.Id).Select(x => new { x.r.Id, x.r.Name, x.r.Description, x.u }).ToList();
            var roles = data.ToList();
            List<RoleUserModel> dataList = new List<RoleUserModel>();
            foreach (Role role in roles)
            {
                RoleUserModel roleUser = new RoleUserModel { Id = role.Id, Description = role.Description, Name = role.Name };
                var users = dataListTemp.Where(x => x.Id == role.Id).Select(x => x.u).ToList();
                foreach (var user in users)
                    roleUser.Users.Add(new RoleUserModel.User { Id = user.ID, UserName = user.Username, FirstName = user.FirstName, LastName = user.LastName });
                dataList.Add(roleUser);
            }
            var dataList1 = dataList.Select(x => new { x.Description, Users = string.Join(", ", x.Users.Select(y => y.UserName)) }).ToList();
            string filePath = Web.Infrastructure.ExporterManager.Export("roleuserlist", Web.Infrastructure.ExporterType.CSV, dataList1, "");
            HttpResponseMessage result = null;

            if (!File.Exists(filePath))
            {
                result = Request.CreateResponse(HttpStatusCode.Gone);
            }
            else
            {
                result = Request.CreateResponse(HttpStatusCode.OK);
                result.Content = new StreamContent(new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
                result.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/csv");
                result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
                result.Content.Headers.ContentDisposition.FileName = Path.GetFileName(filePath);
                result.Content.Headers.ContentLength = new FileInfo(filePath).Length;

            }
            return result;
        }
        // DELETE api/role/5/RemoveUser/3
        [Route("api/roleuserlist/remove/{rid}/{uid}")]
        public IHttpActionResult DeleteUser(Guid rid,Guid uid)
        {
            if (rid == default(Guid))
                return BadRequest("Role id cannot be empty.");
            if (uid == default(Guid))
                return BadRequest("User id cannot be empty.");
            _service.RemoveUsersFromRoles(new List<Guid> { uid }, new List<Guid> { rid });
            return Ok();
        }
    }
}
