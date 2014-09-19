using App.Common.Logging;
using App.Mvc.JqGrid;
using BrockAllen.MembershipReboot.Nh;
using Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Linq.Dynamic;
using Web.Infrastructure.Exceptions;
using WebFramework.Data.Domain;
using App.Common.InversionOfControl;
using System.Text;
using Web.Infrastructure;
using System.IO;

namespace Web.Controllers.Api
{
    [Authorize(Roles = Constants.ROLE_ADMIN)]
    public class LogController : ApiController
    {
        private ILogService _service;
        public LogController()
        {
            _service = IoC.GetService<ILogService>();
        }
        // GET api/log/5
        //public string Get(int id)
        //{
        //    return "value";
        //}

        // POST api/log
        [AllowAnonymous]
        public HttpResponseMessage PostJavascriptLog(LogEntry[] data)
        {
            LogLevel logLevel = LogLevel.Debug;
            foreach (var item in data)
            {
                StringBuilder message = new StringBuilder();
                message.AppendLine(item.Message);
                message.AppendFormat("Request Url: {0}", item.Url);
                Enum.TryParse<LogLevel>(item.Level, true, out logLevel);
                //Logger.Log(logLevel, new JavascriptException(item.Message));
                if (logLevel == LogLevel.Error || logLevel == LogLevel.Fatal)
                    Logger.Log(logLevel, new JavascriptException(message.ToString()));
                else
                    Logger.Log(logLevel, message.ToString());
            }
            var response = Request.CreateResponse(HttpStatusCode.Created);
            return response;

            //LogLevel logLevel = LogLevel.Debug;
            //StringBuilder message = new StringBuilder();
            //message.AppendLine(data.Message);
            //message.AppendFormat("Request Url: {0}", data.Url);
            //Enum.TryParse<LogLevel>(data.Level, true, out logLevel);
            //if (logLevel == LogLevel.Error || logLevel == LogLevel.Fatal)
            //    Logger.Log(logLevel, new JavascriptException(message.ToString()));
            //else
            //    Logger.Log(logLevel, message.ToString());
            //var response = Request.CreateResponse(HttpStatusCode.Created);
            //return response;

        }
        public struct LogEntry
        {
            public string Logger;
            public long Timestamp;
            public string Level;
            public string Url;
            public string Message;
        }
        public dynamic GetGridData([FromUri] JqGridSearchModel searchModel)
        {
            var data = GetQuery(searchModel);
            var dataList = data.Items.Select(x => new { x.Id, x.Application, x.CreatedDate, x.LogLevel, x.UserName, x.Message, x.Host, x.SessionId }).ToList();
            var grid = new JqGridModel
            {
                total = data.TotalPage,
                page = data.CurrentPage,
                records = data.TotalNumber,
                rows = dataList.Select(x => new { id = x.Id, cell = new object[] { x.Id, x.Application, x.CreatedDate, x.LogLevel, x.UserName, x.Message, x.Host, x.SessionId } }).ToArray()
            };

            return grid;
        }
        [Route("api/log/exporttoexcel")]
        [HttpGet]
        public dynamic ExportToExcel([FromUri]JqGridSearchModel searchModel)
        {
            string filePath = null;
            HttpResponseMessage result = null;
            try
            {
                searchModel.rows = 0;
                var data = GetQuery(searchModel);
                var dataList = data.Items.Select(x => new { x.Id, x.Application, x.CreatedDate, x.LogLevel, x.UserName, x.Message, x.Host, x.SessionId }).ToList();
                filePath = ExporterManager.Export("Logs", ExporterType.CSV, dataList.ToList(), "");
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
        private GridModel<Logs> GetQuery([FromUri] JqGridSearchModel searchModel, int maxRecords = Constants.DEFAULT_MAX_RECORDS_RETURN)
        {
            var query = _service.Query();
            if (Constants.SHOULD_FILTER_BY_APP)
                query = query.Where(x => x.Application == App.Common.Util.ApplicationConfiguration.AppAcronym);
            var data = Util.GetGridData<Logs>(searchModel, query);
            return data;
        }
    }
}
