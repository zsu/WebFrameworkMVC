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

namespace Web.Controllers.Api
{
    [Authorize(Roles = Constants.ROLE_ADMIN)]
    public class PermissionController : ApiController
    {
        private IPermissionService _service;
        public PermissionController()
        {
            _service = IoC.GetService<IPermissionService>();
        }
        // GET api/permission
        public dynamic GetGridData([FromUri] JqGridSearchModel searchModel)
        {
            var data = Web.Infrastructure.Util.GetGridData<Permission>(searchModel, _service.Query());
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

        // GET api/permission/5
        public IHttpActionResult Get(Guid id)
        {
            var item = _service.Query().FirstOrDefault((p) => p.Id == id);
            if (item == null)
            {
                return NotFound();
            }
            return Ok(item);
        }

        // POST api/permission
        public IHttpActionResult Post([FromBody] Permission item)
        {
            StringBuilder message = new StringBuilder();
            if (item == null)
                return BadRequest("Permission cannot be empty.");
            if (string.IsNullOrEmpty(item.Name))
                return BadRequest("Permissionname cannot be empty.");
            item.Name = item.Name.Trim();
            item.Description = string.IsNullOrEmpty(item.Description) ? null : item.Description.Trim();
            if (_service.PermissionExists(item.Name))
                return BadRequest(string.Format("Permission {0} already exists.", item.Name));
            _service.CreatePermission(item);
            message.AppendFormat("Permission {0}  is saved successflly.", item.Name);
            return Json<object>(new { Success = true, Message = message.ToString(), RowId = item.Id });
        }

        // PUT api/permission/5
        public IHttpActionResult Put(Guid id, [FromBody] Permission item)
        {
            StringBuilder message = new StringBuilder();
            if (id == default(Guid))
                return BadRequest("Permission id cannot be empty.");
            if (item==null)
            {
                return BadRequest("Permission cannot be empty.");
            }
            if (string.IsNullOrEmpty(item.Name))
            {
                return BadRequest("Permissionname cannot be empty.");
            }
            item.Id = id;
            item.Name = item.Name.Trim();
            item.Description=string.IsNullOrEmpty(item.Description)?null:item.Description.Trim();
            _service.UpdatePermission(item);
            message.AppendFormat("Permission {0}  is saved successflly.", item.Name);
            return Json<object>(new { Success = true, Message = message.ToString(), RowId=item.Id });

        }

        // DELETE api/permission/5
        public IHttpActionResult Delete(Guid id)
        {
            if (id == default(Guid))
                return BadRequest("Permission id cannot be empty.");
            if (!_service.CanDelete(id))
                return BadRequest("Permission is still being used and cannot be deleted.");
            if (_service.DeletePermission(id))
                return Ok();
            else
                return NotFound();
        }

    }
}
