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
using System.IO;

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
        [Route("api/permission/exporttoexcel")]
        [HttpGet]
        public dynamic ExportToExcel([FromUri]JqGridSearchModel searchModel)
        {
            var query = _service.Query();
            //query = query.Where(x => x.Name.StartsWith(string.Format("{0}.", App.Common.Util.ApplicationConfiguration.AppAcronym)));
            searchModel.rows = 0;
            var data = Web.Infrastructure.Util.GetGridData<Permission>(searchModel, query);
            var dataList = data.Items.Select(x => new { x.Name, x.Description }).ToList();
            string filePath = Web.Infrastructure.ExporterManager.Export("permission", Web.Infrastructure.ExporterType.CSV, dataList.ToList(), "");
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
