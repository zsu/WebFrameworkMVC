using App.Common.Logging;
using App.Mvc.JqGrid;
using Service;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Linq.Dynamic;
using Web.Infrastructure.Exceptions;
using WebFramework.Data.Domain;
using App.Common.InversionOfControl;
using Web.Infrastructure;

namespace Web.Controllers.Api
{
    public class ElmahLogController : ApiController
    {
        private IElmahLogService _service;
        public ElmahLogController()
        {
            _service = IoC.GetService<IElmahLogService>();
        }
        // GET api/log/5
        //public string Get(int id)
        //{
        //    return "value";
        //}

        // POST api/log
        public HttpResponseMessage PostJavascriptLog(LogEntry[] data)
        {
            LogLevel logLevel = LogLevel.Debug;
            foreach (var item in data)
            {
                Enum.TryParse<LogLevel>(item.Level, true, out logLevel);
                Logger.Log(logLevel,new JavascriptException(item.Message));
            }
            var response = Request.CreateResponse(HttpStatusCode.Created);
            return response;

        }
        public struct LogEntry
        {
            public string Logger;
            public long Timestamp;
            public string Level;
            public string Url;
            public string Message;
        }
        [Authorize(Roles = Constants.ROLE_ADMIN)]
        public dynamic GetGridData([FromUri] JqGridSearchModel searchModel)
        {
            var data = Util.GetGridData<ElmahLog>(searchModel, _service.Query());
            var dataList = data.Items.Select(x => new { x.Id, x.Application, x.Time, x.Type, x.User, x.Message, x.Host, x.StatusCode }).ToList();
            var grid = new JqGridModel
            {
                total = data.TotalPage,
                page = data.CurrentPage,
                records = data.TotalNumber,
                rows = dataList.Select(x => new { id = x.Id, cell = new object[] { x.Id, x.Application, x.Time, x.Type, x.User, x.Message, x.Host, x.StatusCode } }).ToArray()
            };
            return grid;
        }
    }
}
