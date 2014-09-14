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
using WebFramework.Data.Domain;
using App.Common.Tasks;
using System.IO;

namespace Web.Controllers.Api
{
    [Authorize(Roles = Constants.ROLE_ADMIN)]
    public class TaskController : ApiController
    {
        private ITaskService _service;
        public TaskController()
        {
            _service = IoC.GetService<ITaskService>();
        }
        // GET api/task
        public dynamic GetGridData([FromUri] JqGridSearchModel searchModel)
        {
            var data = Web.Infrastructure.Util.GetGridData<ScheduleTask>(searchModel, _service.Query());
            var dataList = data.Items.ToList();
            var grid = new JqGridModel
            {
                total = data.TotalPage,
                page = data.CurrentPage,
                records = data.TotalNumber,
                rows = dataList.Select(x => new { id = x.Id, cell = new object[] { x.Id, x.Name, x.Seconds, x.Type, x.Enabled, x.StopOnError, x.LastStartUtc, x.LastEndUtc, x.LastSuccessUtc } }).ToArray()
            };
            return grid;
        }
        [Route("api/task/exporttoexcel")]
        [HttpGet]
        public dynamic ExportToExcel([FromUri]JqGridSearchModel searchModel)
        {
            string filePath = null;
            HttpResponseMessage result = null;
            try
            {
                var query = _service.Query();
                searchModel.rows = 0;
                var data = Web.Infrastructure.Util.GetGridData<ScheduleTask>(searchModel, query);
                var dataList = data.Items.Select(x => new { x.Id, x.Name, x.Seconds, x.Type, x.Enabled, x.StopOnError, x.LastStartUtc, x.LastEndUtc, x.LastSuccessUtc }).ToList();
                filePath = Web.Infrastructure.ExporterManager.Export("task", Web.Infrastructure.ExporterType.CSV, dataList.ToList(), "");
            }
            catch (Exception ex)
            {
                return Web.Infrastructure.Util.DisplayExportError(ex);
            }
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
        // GET api/task/5
        public IHttpActionResult Get(Guid id)
        {
            var item = _service.Query().FirstOrDefault((p) => p.Id == id);
            if (item == null)
            {
                return NotFound();
            }
            return Ok(item);
        }

        // POST api/task
        public IHttpActionResult Post([FromBody] ScheduleTask item)
        {
            StringBuilder message = new StringBuilder();
            if (item == null)
                return BadRequest("Task cannot be empty.");
            if (string.IsNullOrEmpty(item.Name))
                return BadRequest("Task name cannot be empty.");
            item.Name = item.Name.Trim();
            if (_service.Exists(item.Name))
                return BadRequest(string.Format("Permission {0} already exists.", item.Name));
            _service.Add(item);
            message.AppendFormat("Task {0}  is saved successflly.", item.Name);
            return Json<object>(new { Success = true, Message = message.ToString(), RowId = item.Id });
        }

        // PUT api/task/5
        public IHttpActionResult Put(Guid id, [FromBody] ScheduleTask item)
        {
            StringBuilder message = new StringBuilder();
            if (id == default(Guid))
                return BadRequest("Task id cannot be empty.");
            if (item == null)
            {
                return BadRequest("Task cannot be empty.");
            }
            if (string.IsNullOrEmpty(item.Name))
            {
                return BadRequest("Task name cannot be empty.");
            }
            item.Id = id;
            item.Name = item.Name.Trim();
            _service.Update(item);
            message.AppendFormat("Task {0}  is saved successflly.", item.Name);
            return Json<object>(new { Success = true, Message = message.ToString(), RowId = item.Id });

        }

        // DELETE api/task/5
        public IHttpActionResult Delete(Guid id)
        {
            if (id == default(Guid))
                return BadRequest("Task id cannot be empty.");
            if (_service.Delete(id))
                return Ok();
            else
                return NotFound();
        }

    }
}
