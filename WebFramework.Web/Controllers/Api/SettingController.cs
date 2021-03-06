﻿using Service;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Linq.Dynamic;
using App.Common.InversionOfControl;
using App.Mvc.JqGrid;
using System.Text;
using WebFramework.Data.Domain;
using System.IO;

namespace Web.Controllers.Api
{
    [Authorize(Roles = Constants.ROLE_ADMIN)]
    public class SettingController : ApiController
    {
        private ISettingService _service;
        public SettingController()
        {
            _service = IoC.GetService<ISettingService>();
        }
        // GET api/setting
        public dynamic GetGridData([FromUri] JqGridSearchModel searchModel)
        {
            var data = GetQuery(_service.Query(),searchModel);
            var dataList = data.Items.Select(x => new { x.Id, x.Name, x.Value }).ToList();
            var grid = new JqGridModel
            {
                total = data.TotalPage,
                page = data.CurrentPage,
                records = data.TotalNumber,
                rows = dataList.Select(x => new { id = x.Id, cell = new object[] { x.Id, x.Name, x.Value } }).ToArray()
            };
            return grid;
        }
        [Route("api/setting/exporttoexcel")]
        [HttpGet]
        public dynamic ExportToExcel([FromUri]JqGridSearchModel searchModel)
        {
            string filePath = null;
            HttpResponseMessage result = null;
            try
            {
                searchModel.rows = 0;
                var data = GetQuery(Web.Infrastructure.Util.GetStatelessQuery<Setting>(),searchModel);
                var dataList = data.Items.Select(x => new { x.Name, x.Value }).ToList();
                filePath = Web.Infrastructure.ExporterManager.Export("setting", Web.Infrastructure.ExporterType.CSV, dataList.ToList(), "");
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
        // GET api/setting/5
        public IHttpActionResult Get(Guid id)
        {
            var item = _service.Query().FirstOrDefault((p) => p.Id == id);
            if (item == null)
            {
                return NotFound();
            }
            return Ok(item);
        }

        // POST api/setting
        public IHttpActionResult Post([FromBody] Setting item)
        {
            StringBuilder message = new StringBuilder();
            if (item == null)
                return BadRequest("Setting cannot be empty.");
            if (string.IsNullOrEmpty(item.Name))
                return BadRequest("Setting name cannot be empty.");
            if (string.IsNullOrEmpty(item.Value))
                return BadRequest("Setting value cannot be empty.");
            item.Name = item.Name.Trim();
            if (Constants.SHOULD_FILTER_BY_APP && !item.Name.StartsWith(App.Common.Util.ApplicationConfiguration.AppAcronym))
                return BadRequest(string.Format("Name must start with '{0}.'", App.Common.Util.ApplicationConfiguration.AppAcronym));
            item.Value = string.IsNullOrEmpty(item.Value) ? null : item.Value.Trim();
            if (_service.SettingExists(item.Name))
                return BadRequest(string.Format("Permission {0} already exists.", item.Name));
            _service.AddSetting(item);
            message.AppendFormat("Setting {0}  is saved successflly.", item.Name);
            return Json<object>(new { Success = true, Message = message.ToString(), RowId = item.Id });
        }

        // PUT api/setting/5
        public IHttpActionResult Put(Guid id, [FromBody] Setting item)
        {
            StringBuilder message = new StringBuilder();
            if (id == default(Guid))
                return BadRequest("Setting id cannot be empty.");
            if (item == null)
            {
                return BadRequest("Setting cannot be empty.");
            }
            if (string.IsNullOrEmpty(item.Name))
            {
                return BadRequest("Setting name cannot be empty.");
            }
            if (string.IsNullOrEmpty(item.Value))
            {
                return BadRequest("Setting value cannot be empty.");
            }
            item.Id = id;
            item.Name = item.Name.Trim();
            item.Value = string.IsNullOrEmpty(item.Value) ? null : item.Value.Trim();
            _service.UpdateSetting(item);
            message.AppendFormat("Setting {0}  is saved successflly.", item.Name);
            return Json<object>(new { Success = true, Message = message.ToString(), RowId = item.Id });

        }

        // DELETE api/setting/5
        public IHttpActionResult Delete(Guid id)
        {
            if (id == default(Guid))
                return BadRequest("Setting id cannot be empty.");
            if (_service.DeleteSetting(id))
                return Ok();
            else
                return NotFound();
        }
        private Web.Infrastructure.GridModel<Setting> GetQuery(IQueryable<Setting> query,[FromUri] JqGridSearchModel searchModel, int maxRecords = Constants.DEFAULT_MAX_RECORDS_RETURN)
        {
            if (Constants.SHOULD_FILTER_BY_APP)
                query = query.Where(x => x.Name.StartsWith(string.Format("{0}.", App.Common.Util.ApplicationConfiguration.AppAcronym)));
            var data = Web.Infrastructure.Util.GetGridData<Setting>(searchModel, query);
            return data;
        }
    }
}
